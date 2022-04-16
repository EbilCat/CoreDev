using System.Collections.Generic;
using CoreDev.Observable;
using UnityEngine;

public class ODictionaryTester : MonoBehaviour
{
    private ODictionary<string, string> dictionary;
    private string elementsAdded;
    private string elementsRemoved;


    [ContextMenu("RunTests")]
    public void RunTests()
    {
        this.Test_Add();
        this.Test_Remove();
        this.Test_Clear();
        this.Test_Moderator_PreventAddition();
        this.Test_Moderator_PreventRemoval();
        this.Test_Moderator_EditValues();
    }

    public void Test_Add()
    {
        this.SetupTest();

        this.dictionary.Add("Ada", "Wong");
        this.dictionary.Add("Jill", "Valentine");
        this.dictionary.Add("Claire", "Redfield");
        this.dictionary.Add("Rebecca", "Chambers");
        this.dictionary.Add("Sherry", "Birkin");

        this.ValidateListChanges(
            "Test_Add", 
            "Ada:Wong,Jill:Valentine,Claire:Redfield,Rebecca:Chambers,Sherry:Birkin", 
            "Ada:Wong,Jill:Valentine,Claire:Redfield,Rebecca:Chambers,Sherry:Birkin", 
            string.Empty);
    }
    public void Test_Remove()
    {
        this.SetupTest();
        this.dictionary.Add("Ada", "Wong");
        this.dictionary.Add("Jill", "Valentine");
        this.dictionary.Add("Claire", "Redfield");
        this.dictionary.Add("Rebecca", "Chambers");
        this.dictionary.Add("Sherry", "Birkin");

        dictionary.Remove("Claire");

        this.ValidateListChanges(
            "Test_Remove", 
            "Ada:Wong,Jill:Valentine,Rebecca:Chambers,Sherry:Birkin", 
            "Ada:Wong,Jill:Valentine,Claire:Redfield,Rebecca:Chambers,Sherry:Birkin", 
            "Claire:Redfield");
    }
    public void Test_Clear()
    {
        this.SetupTest();
        this.dictionary.Add("Ada", "Wong");
        this.dictionary.Add("Jill", "Valentine");
        this.dictionary.Add("Claire", "Redfield");
        this.dictionary.Add("Rebecca", "Chambers");
        this.dictionary.Add("Sherry", "Birkin");

        dictionary.Clear();

        this.ValidateListChanges(
            "Test_Clear", 
            string.Empty,
            "Ada:Wong,Jill:Valentine,Claire:Redfield,Rebecca:Chambers,Sherry:Birkin", 
            "Ada:Wong,Jill:Valentine,Claire:Redfield,Rebecca:Chambers,Sherry:Birkin");
    }
    public void Test_Moderator_PreventAddition()
    {
        this.SetupTest();
        this.dictionary.Add("Ada", "Wong");
        this.dictionary.Add("Jill", "Valentine");
        this.dictionary.AddModerator(PreventAdding);
        this.dictionary.Add("Claire", "Redfield");
        this.dictionary.Add("Rebecca", "Chambers");
        this.dictionary.Add("Sherry", "Birkin");

        this.ValidateListChanges("Test_Moderator_PreventAddition", "Ada:Wong,Jill:Valentine", "Ada:Wong,Jill:Valentine", string.Empty);
    }

    public void Test_Moderator_PreventRemoval()
    {
        this.SetupTest();
        this.dictionary.Add("Ada", "Wong");
        this.dictionary.Add("Jill", "Valentine");

        this.dictionary.AddModerator(PreventRemoving);
        this.dictionary.Remove("Jill");

        dictionary.Clear();

        this.ValidateListChanges("Test_Moderator_PreventRemoval", "Ada:Wong,Jill:Valentine", "Ada:Wong,Jill:Valentine", string.Empty);
    }

    public void Test_Moderator_EditValues()
    {
        this.SetupTest();

        this.dictionary.AddModerator(RemoveAllLetterA);
        this.dictionary.Add("Ada", "Wong");

        this.ValidateListChanges("Test_Moderator_EditValues", "d:Wong", "d:Wong", string.Empty);
    }


//*====================
//* CALLBACKS
//*====================
    private void OnElementAdded(ODictionary<string, string> list, string key, string addedElement)
    {
        if (this.elementsAdded.Length > 0)
        {
            this.elementsAdded += ",";
        }
        this.elementsAdded += $"{key}:{addedElement}";
    }


    private void OnElementRemoved(ODictionary<string, string> list, string key, string removedElement)
    {
        if (this.elementsRemoved.Length > 0)
        {
            this.elementsRemoved += ",";
        }
        this.elementsRemoved += $"{key}:{removedElement}";
    }


//*====================
//* PRIVATE
//*====================
    private void SetupTest()
    {
        this.dictionary = new ODictionary<string, string>();
        this.elementsAdded = string.Empty;
        this.elementsRemoved = string.Empty;

        this.dictionary.RegisterForElementAdded(OnElementAdded, false);
        this.dictionary.RegisterForElementRemoved(OnElementRemoved);
    }

    private void ValidateListChanges(string testName, string expectedContent, string expectedAdditions, string expectedRemovals)
    {
        string content = string.Empty;

        foreach (KeyValuePair<string, string> kvp in dictionary)
        {
            if (content.Length > 0)
            {
                content += ",";
            }
            content += $"{kvp.Key}:{kvp.Value}";
        }

        bool contentsValidated = string.Equals(content, expectedContent);

        if (contentsValidated)
        {
            Debug.Log($"{testName} (Contents): Passed");
        }
        else
        {
            Debug.LogWarning($"{testName} (Contents): Failed\r\nExpected:{expectedContent}\r\nActual:{content}\r\n\r\n");
        }


        bool additionsValidated = string.Equals(this.elementsAdded, expectedAdditions);

        if (additionsValidated)
        {
            Debug.Log($"{testName} (Additions): Passed");
        }
        else
        {
            Debug.LogWarning($"{testName} (Additions): Failed\r\nExpected:{expectedAdditions}\r\nActual:{elementsAdded}\r\n\r\n");
        }


        bool removalsValidated = string.Equals(this.elementsRemoved, expectedRemovals);

        if (removalsValidated)
        {
            Debug.Log($"{testName} (Removals): Passed");
        }
        else
        {
            Debug.LogWarning($"{testName} (Removals): Failed\r\nExpected:{expectedRemovals}\r\nActual:{elementsRemoved}\r\n\r\n");
        }
    }


//*====================
//* MODERATORS
//*====================
    private bool RemoveAllLetterA(ref string key, ref string incomingValue, ODictionaryOperation op)
    {
        if (op == ODictionaryOperation.ADD)
        {
            key = key.Replace("A", string.Empty);
            key = key.Replace("a", string.Empty);
            incomingValue = incomingValue.Replace("A", string.Empty);
            incomingValue = incomingValue.Replace("a", string.Empty);
        }
        return true;
    }

    private bool RejectValuesContainingLetterO(ref string key, ref string incomingValue, ODictionaryOperation op)
    {
        if (op == ODictionaryOperation.ADD && incomingValue.Contains("O"))
        {
            return false;
        }
        return true;
    }

    private bool PreventAdding(ref string key, ref string incomingValue, ODictionaryOperation op)
    {
        if (op == ODictionaryOperation.ADD)
        {
            return false;
        }
        return true;
    }

    private bool PreventRemoving(ref string key, ref string incomingValue, ODictionaryOperation op)
    {
        if (op == ODictionaryOperation.REMOVE)
        {
            return false;
        }
        return true;
    }
}