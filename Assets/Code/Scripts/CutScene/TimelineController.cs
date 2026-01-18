using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public static bool isTimelinePlaying;

    PlayableDirector director;

    public GameObject objectToEnable;

    void Start()
    {
        director.Play();
    }

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    void OnEnable()
    {
        director.played += OnTimelineStart;
        director.stopped += OnTimelineEnd;
    }

    void OnDisable()
    {
        director.played -= OnTimelineStart;
        director.stopped -= OnTimelineEnd;
    }

    void OnTimelineStart(PlayableDirector d)
    {
        isTimelinePlaying = true;
    }

    void OnTimelineEnd(PlayableDirector d)
    {
        isTimelinePlaying = false;

        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }
    }
}
