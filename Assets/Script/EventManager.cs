using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public float currentEventCooldown = 0;

    public EventData[] events;

    [Tooltip("How long to wait before this becomes active.")]
    public float firstTrigerDelay = 180f;

    [Tooltip("How long to wait between each event.")]
    public float triggerInterval = 30f;

    public static EventManager instance;

    [System.Serializable]
    public class Event
    {
        public EventData data;
        public float duration, cooldown = 0;
    }
    List<Event> runningEvents = new List<Event>();

    PlayerStats[] allPlayers;

    private void Start()
    {
        if (instance) Debug.LogWarning("There is more than one Spawn Manager in the scene! Please remove the extras.");
        instance = this;
        currentEventCooldown = firstTrigerDelay > 0 ? firstTrigerDelay : triggerInterval;
        allPlayers = FindObjectsOfType<PlayerStats>();

        if (allPlayers.Length == 0)
        {
            Debug.LogWarning("No players found in the scene!");
        }
    }

    public void Update()
    {
        currentEventCooldown -= Time.deltaTime;
        if (currentEventCooldown <= 0)
        {
            EventData e = GetRandomEvent();
            if (e && e.CheckIfWillHappen(allPlayers[Random.Range(0, allPlayers.Length)]))
                runningEvents.Add(new Event
                {
                    data = e,
                    duration = e.duration
                });
            currentEventCooldown = triggerInterval;
        }

        List<Event> toRemove = new List<Event>();

        foreach (Event e in runningEvents)
        {
            e.duration -= Time.deltaTime;
            if (e.duration <= 0)
            {
                toRemove.Add(e);
                continue;
            }

            e.cooldown -= Time.deltaTime;
            if (e.cooldown <= 0)
            {
                e.data.Activate(allPlayers[Random.Range(0, allPlayers.Length)]);
                e.cooldown = e.data.GetSpawnInterval();
            }
        }

        foreach (Event e in toRemove) runningEvents.Remove(e);
    }

    public EventData GetRandomEvent()
    {
        if (events.Length <= 0) return null;

        List<EventData> possibleEvents = new List<EventData>();

        foreach (EventData e in events)
        {
            if (e.IsActive())
            {
                possibleEvents.Add(e);
            }
        }

        if (possibleEvents.Count > 0)
        {
            EventData result = possibleEvents[Random.Range(0, possibleEvents.Count)];
            return result;
        }

        Debug.LogWarning("No active events available to trigger.");
        return null;
    }
}
