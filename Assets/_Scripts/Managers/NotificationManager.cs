using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    private Transform notificationsUIEmplacement;
    public GameObject notificationPrefab;
    public static NotificationManager Instance;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        notificationsUIEmplacement = InterfaceManager.Instance.GetUI().transform.Find("Notifications");
    }

    public void Notify(Upgrade upgrade)
    {
        GameObject newNotification = Instantiate(notificationPrefab, notificationsUIEmplacement);
        newNotification.name = upgrade.name;
        newNotification.GetComponent<Image>().sprite = upgrade.GetArtwork();
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 50), "Add Notification"))
            Notify(UnitManager.Instance.GetRandomUnit().GetRandomSpell(includingAttack:true));
    }
}
