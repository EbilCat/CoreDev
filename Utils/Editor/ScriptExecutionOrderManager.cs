#if UNITY_EDITOR
#define LOG_SCRIPTORDER
// #define LOG_CLASSVALIDITY

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CoreDev.Utils
{
    [InitializeOnLoad]
    public class ScriptExecutionOrderManager : Editor
    {
        static ScriptExecutionOrderManager()
        {
            ReorderScripts();
        }

        public static void ReorderScripts()
        {
            if (EditorApplication.isCompiling || EditorApplication.isPaused || EditorApplication.isPlaying)
            {
                return;
            }

            Dictionary<string, int> executionOrderLog = new Dictionary<string, int>();
            List<string> validScriptNames = new List<string>();
            int scriptsCounter = 0;

#if LOG_CLASSVALIDITY
            List<string> nullScriptInfo = new List<string>();
            int nullCounter = 0;
#endif

            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                Type type = monoScript.GetClass();
                if (type != null)
                {
                    bool hasExecutionAttr = false;
                    int newExecutionIndex = 0;
                    foreach (Attribute attr in Attribute.GetCustomAttributes(type, typeof(ScriptExecutionOrder)))
                    {
                        hasExecutionAttr = true;
                        newExecutionIndex = (attr as ScriptExecutionOrder).executionIndex;
                        executionOrderLog.Add(type.ToString(), newExecutionIndex);
                    }
                    int currentExecutionIndex = MonoImporter.GetExecutionOrder(monoScript);
                    if (hasExecutionAttr && currentExecutionIndex != newExecutionIndex)
                    {
                        MonoImporter.SetExecutionOrder(monoScript, newExecutionIndex);
                    }

                    scriptsCounter++;

                    validScriptNames.Add(type.ToString());
                }
                else
                {
#if LOG_CLASSVALIDITY
                    nullCounter++;

                    MatchCollection matches = Regex.Matches(@monoScript.ToString(), @"(?:class|namespace|interface)(\s)(\w+)(?:<[\w\s,]+>)?", RegexOptions.Multiline);

                    String info = string.Empty;
                    string namespaces = string.Empty;
                    string classes = string.Empty;
                    string interfaces = string.Empty;
                    foreach (var match in matches)
                    {
                        if (Regex.IsMatch(@match.ToString(), @"namespace"))
                        {
                            if (namespaces != string.Empty) { namespaces += "."; }
                            namespaces += match.ToString().Replace("namespace", "").Trim();
                        }
                        if (Regex.IsMatch(@match.ToString(), @"interface"))
                        {
                            if (interfaces != string.Empty) { interfaces += ", "; }
                            interfaces += match.ToString().Replace("interface", "").Trim();
                        }
                        if (Regex.IsMatch(@match.ToString(), @"class"))
                        {
                            if (classes != string.Empty) { classes += ", "; }
                            classes += match.ToString().Replace("class", "").Trim();
                        }
                    }
                    if (namespaces != string.Empty) { info += "\r\n\tNamespace - " + namespaces; }
                    if (interfaces != string.Empty) { info += "\r\n\tInterface - " + interfaces; }
                    if (classes != string.Empty) { info += "\r\n\tClass - " + classes; }
                    if (info != string.Empty) { nullScriptInfo.Add(string.Format("{0}: {1}", monoScript.name, info)); }
                    else
                    {
                        if (monoScript.ToString() == string.Empty) { nullScriptInfo.Add(monoScript.name + ": Empty File"); }
                        else { nullScriptInfo.Add(monoScript.name); }
                    }
#endif
                }
            }
#if LOG_SCRIPTORDER
            if (executionOrderLog.Count > 0)
            {
                string header = "No. of scripts with ScriptExecutionOrder attribute: " + executionOrderLog.Count;
                string values = string.Empty;
                foreach (KeyValuePair<string, int> item in executionOrderLog.OrderBy(key => key.Value))
                {
                    values += string.Format("{0}: {1}\r\n", item.Value, item.Key);
                }
                Debug.Log(string.Format("{0}: {1}\r\n{2}", typeof(ScriptExecutionOrderManager).Name, header, values));
            }
#endif

#if LOG_CLASSVALIDITY
            validScriptNames.Sort();
            string scriptsHeader = string.Format("No. of Monoscripts with valid GetClass(): {0}/{1}", scriptsCounter, scriptsCounter + nullCounter);
            string scriptsNames = string.Empty;
            foreach (string name in validScriptNames) { scriptsNames += (name + "\r\n"); }
            Debug.Log(string.Format("{0}: {1}\r\n{2}", typeof(ScriptExecutionOrderManager).Name, scriptsHeader, scriptsNames));

            if (nullCounter > 0)
            {
                nullScriptInfo.Sort(delegate (string first, string second) { return first.Length.CompareTo(second.Length); });
                string nullHeader = "No. of Monoscripts with null GetClass(): " + nullCounter;
                string nullCause = "Possible cause: Generic class / Monoscript filename and Class name mismatch\r\n";
                foreach (string name in nullScriptInfo) { nullCause += (name + "\r\n"); }
                Debug.Log(string.Format("{0}: {1}\r\n{2}", typeof(ScriptExecutionOrderManager).Name, nullHeader, nullCause));
            }
#endif
        }
    }
}
#endif