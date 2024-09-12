using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject journalPanel;
    public GameObject sideBySideLayout; 
    
    public void ShowInventoryOnly()
    {
       // DeactivateAllPanels();
        inventoryPanel.SetActive(true);
        Debug.Log("Showing Inventory Only");
    }

    public void ShowJournalOnly()
    {
        //DeactivateAllPanels();
        journalPanel.SetActive(true);
        Debug.Log("Showing Journal Only");
    }

    public void ShowSideBySide()
    {
        //DeactivateAllPanels();
        inventoryPanel.SetActive(true);
        journalPanel.SetActive(true);
        sideBySideLayout.SetActive(true);
        Debug.Log("Showing Side by Side");
    }

   // private void DeactivateAllPanels()
 //   {
   //     inventoryPanel.SetActive(false);
   //     journalPanel.SetActive(false);
    //    sideBySideLayout.SetActive(false);
   // }
//}
}