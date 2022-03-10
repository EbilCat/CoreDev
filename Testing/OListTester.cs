using System;
using System.Collections.Generic;
using CoreDev.Observable;
using UnityEngine;

public class OListTester : MonoBehaviour
{
    private OList<string> listOfNames;
    private string elementsAdded;
    private string elementsRemoved;


    [ContextMenu("RunTests")]
    public void RunTests()
    {
        this.Test_Add();
        this.Test_AddRange();
        this.Test_Insert();
        this.Test_InsertRange();
        this.Test_InsertRangeWithAddPreventionModerator();
        this.Test_Remove();
        this.Test_RemoveAll();
        this.Test_RemoveAt();
        this.Test_RemoveRange();
        this.Test_ReverseWithIndexAndCount();
        this.Test_Reverse();
        this.Test_SortWithComparison();
        this.Test_SortByIndexCountAndComparer();
        this.Test_Sort();
        this.Test_SortWithComparer();
        this.Test_Clear();
        this.Test_Moderator_PreventAddition();
        this.Test_Moderator_PreventRemoval();
        this.Test_Moderator_EditValues();
    }

    public void Test_Add()
    {
        this.SetupTest();

        this.listOfNames.Add("Ada");
        this.listOfNames.Add("Beth");
        this.listOfNames.Add("Charlie");
        this.listOfNames.Add("Darlene");
        this.listOfNames.Add("Erin");

        this.ValidateListChanges("Test_Add", "Ada,Beth,Charlie,Darlene,Erin", "Ada,Beth,Charlie,Darlene,Erin", string.Empty);
    }


    public void Test_AddRange()
    {
        this.SetupTest();

        string[] nameArray = new string[] { "Ada", "Beth", "Charlie", "Darlene", "Erin" };
        this.listOfNames.AddRange(nameArray);

        this.ValidateListChanges("Test_AddRange", "Ada,Beth,Charlie,Darlene,Erin", "Ada,Beth,Charlie,Darlene,Erin", string.Empty);
    }

    public void Test_Insert()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        listOfNames.Insert(1, "BOB");

        this.ValidateListChanges("Test_Insert", "Ada,BOB,Beth,Charlie,Darlene,Erin", "BOB", string.Empty);
    }

    public void Test_InsertRange()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        string[] nameArray = new string[] { "BOB", "CARL" };

        listOfNames.InsertRange(1, nameArray);

        this.ValidateListChanges("Test_InsertRange", "Ada,BOB,CARL,Beth,Charlie,Darlene,Erin", "BOB,CARL", string.Empty);
    }

    public void Test_InsertRangeWithAddPreventionModerator()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        this.listOfNames.AddModerator(RejectValuesContainingLetterO);

        string[] nameArray = new string[] { "BOB", "TOD", "CARL" };

        listOfNames.InsertRange(4, nameArray);

        this.ValidateListChanges("Test_InsertRangeWithAddPreventionModerator", "Ada,Beth,Charlie,Darlene,CARL,Erin", "CARL", string.Empty);
    }

    public void Test_Remove()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        listOfNames.Remove("Beth");

        this.ValidateListChanges("Test_Remove", "Ada,Charlie,Darlene,Erin", string.Empty, "Beth");
    }

    public void Test_RemoveAll()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        Predicate<string> predicate = (x) => x.Contains("a");
        int removalCount = listOfNames.RemoveAll(predicate);
        this.ValidateListChanges("Test_RemoveAll", "Beth,Erin", string.Empty, "Ada,Charlie,Darlene");

        if (removalCount == 3)
        {
            Debug.Log("Test_RemoveAll: RemovalCount Passed");
        }
        else
        {
            Debug.Log($"Test_RemoveAll: RemovalCount Failed\r\nExpected:3\r\nActual:{removalCount}\r\n\r\n");
        }
    }

    public void Test_RemoveAt()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        listOfNames.RemoveAt(1);

        this.ValidateListChanges("Test_RemoveAt", "Ada,Charlie,Darlene,Erin", string.Empty, "Beth");
    }

    public void Test_RemoveRange()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        listOfNames.RemoveRange(1, 2);

        this.ValidateListChanges("Test_RemoveRange", "Ada,Darlene,Erin", string.Empty, "Beth,Charlie");
    }

    public void Test_ReverseWithIndexAndCount()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        listOfNames.Reverse(1, 3);

        this.ValidateListChanges("Test_ReverseWithIndexAndCount", "Ada,Darlene,Charlie,Beth,Erin", string.Empty, string.Empty);
    }

    public void Test_Reverse()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        listOfNames.Reverse();

        this.ValidateListChanges("Test_Reverse", "Erin,Darlene,Charlie,Beth,Ada", string.Empty, string.Empty);
    }

    public void Test_SortWithComparison()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        Comparison<string> comparison = (x, y) => y[0] - x[0];

        listOfNames.Sort(comparison);

        this.ValidateListChanges("Test_SortWithComparison", "Erin,Darlene,Charlie,Beth,Ada", string.Empty, string.Empty);
    }

    public void Test_SortByIndexCountAndComparer()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        listOfNames.Sort(0, 3, new Comparer());

        this.ValidateListChanges("Test_SortByIndexCountAndComparer", "Charlie,Beth,Ada,Darlene,Erin", string.Empty, string.Empty);
    }

    public void Test_Sort()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        listOfNames.Sort();

        this.ValidateListChanges("Test_Sort", "Ada,Beth,Charlie,Darlene,Erin", string.Empty, string.Empty);
    }

    public void Test_SortWithComparer()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        listOfNames.Sort(new Comparer());

        this.ValidateListChanges("Test_SortWithComparison", "Erin,Darlene,Charlie,Beth,Ada", string.Empty, string.Empty);
    }

    public void Test_Clear()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        listOfNames.Clear();

        this.ValidateListChanges("Test_Clear", string.Empty, string.Empty, "Ada,Beth,Charlie,Darlene,Erin");
    }
    public void Test_Moderator_PreventAddition()
    {
        this.SetupTest("Ada", "Beth");

        this.listOfNames.AddModerator(PreventAddingToList);

        this.listOfNames.Add("Charlie");
        this.listOfNames.Add("Darlene");
        this.listOfNames.Add("Erin");

        this.ValidateListChanges("Test_Moderator_PreventAddition", "Ada,Beth", string.Empty, string.Empty);
    }

    public void Test_Moderator_PreventRemoval()
    {
        this.SetupTest("Ada", "Beth", "Charlie", "Darlene", "Erin");

        this.listOfNames.AddModerator(PreventRemovingFromList);

        listOfNames.Clear();

        this.ValidateListChanges("Test_Moderator_PreventRemoval", "Ada,Beth,Charlie,Darlene,Erin", string.Empty, string.Empty);
    }

    public void Test_Moderator_EditValues()
    {
        this.SetupTest();
        
        this.listOfNames.AddModerator(RemoveAllLetterA);
        this.listOfNames.Add("Ada");
        this.listOfNames.Add("Beth");
        this.listOfNames.Add("Charlie");
        this.listOfNames.Add("Darlene");
        this.listOfNames.Add("Erin");

        this.ValidateListChanges("Test_Moderator_EditValues", "d,Beth,Chrlie,Drlene,Erin", "d,Beth,Chrlie,Drlene,Erin", string.Empty);
    }


    //*====================
    //* CALLBACKS
    //*====================
    private void OnElementAdded(OList<string> list, string addedElement)
    {
        if (this.elementsAdded.Length > 0)
        {
            this.elementsAdded += ",";
        }
        this.elementsAdded += addedElement;
    }


    private void OnElementRemoved(OList<string> list, string removedElement)
    {
        if (this.elementsRemoved.Length > 0)
        {
            this.elementsRemoved += ",";
        }
        this.elementsRemoved += removedElement;
    }


//*====================
//* PRIVATE
//*====================
    private void SetupTest(params string[] contents)
    {
        this.listOfNames = new OList<string>(contents);
        this.elementsAdded = string.Empty;
        this.elementsRemoved = string.Empty;

        this.listOfNames.RegisterForElementAdded(OnElementAdded, false);
        this.listOfNames.RegisterForElementRemoved(OnElementRemoved);
    }
    private void ValidateListChanges(string testName, string expectedContent, string expectedAdditions, string expectedRemovals)
    {
        string content = string.Empty;

        foreach (string name in listOfNames)
        {
            if (content.Length > 0)
            {
                content += ",";
            }
            content += name;
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
    private bool RemoveAllLetterA(ref string incomingValue, OListOperation op)
    {
        if(op == OListOperation.ADD)
        {
            incomingValue = incomingValue.Replace("A", string.Empty);
            incomingValue = incomingValue.Replace("a", string.Empty);
        }
        return true;
    }

    private bool RejectValuesContainingLetterO(ref string incomingValue, OListOperation op)
    {
        if (op == OListOperation.ADD && incomingValue.Contains("O"))
        {
            return false;
        }
        return true;
    }
    
    private bool PreventAddingToList(ref string incomingValue, OListOperation op)
    {
                if (op == OListOperation.ADD)
                {
                    return false;
                }
                return true;
    }

    private bool PreventRemovingFromList(ref string incomingValue, OListOperation op)
    {
        if (op == OListOperation.REMOVE)
        {
            return false;
        }
        return true;
    }


//*====================
//* CLASSES
//*====================
    public class Comparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return y[0] - x[0];
        }
    }
}
