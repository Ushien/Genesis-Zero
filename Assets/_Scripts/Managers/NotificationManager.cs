using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    public void Notify(BaseSpell spell)
    {
        GameObject newNotification = Instantiate(notificationPrefab, notificationsUIEmplacement);
        newNotification.name = spell.name;
    }

    public void Notify()
    {
        GameObject newNotification = Instantiate(notificationPrefab, notificationsUIEmplacement);
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 50), "Add Notification"))
            Notify();
    }
}
