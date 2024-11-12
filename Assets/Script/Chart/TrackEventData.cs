using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TrackEventData 
{
    public KoreographyTrack lineMoveEvent;
    public KoreographyTrack lineDisappearEvent;
    public KoreographyTrack lineRotateEvent;
    public KoreographyTrack aboveNoteEvent;
    public KoreographyTrack belowNoteEvent;
    public KoreographyTrack speedEvent;

    private static readonly TrackEventData _instance = new TrackEventData();
    public static TrackEventData Instance
    {
        get
        {
            return _instance;
        }
    }

    private TrackEventData()
    {
        lineMoveEvent = Resources.Load("Track/LineMove") as KoreographyTrack;
        lineDisappearEvent = Resources.Load("Track/LineDisappear") as KoreographyTrack;
        lineRotateEvent = Resources.Load("Track/LineRotate") as KoreographyTrack;
        aboveNoteEvent = Resources.Load("Track/AboveNote") as KoreographyTrack;
        belowNoteEvent = Resources.Load("Track/BelowNote") as KoreographyTrack;
        speedEvent = Resources.Load("Track/SpeedEvent") as KoreographyTrack;
    }

    public void AddLineMove(KoreographyEvent trackEvent)
    {
        lineMoveEvent.AddEvent(trackEvent);
    }
    public void AddLineDisappear(KoreographyEvent trackEvent)
    {
        lineDisappearEvent.AddEvent(trackEvent);
    }
    public void AddLineRotate(KoreographyEvent trackEvent)
    {
        lineRotateEvent.AddEvent(trackEvent);
    }
    public void AddNote(KoreographyEvent trackEvent)
    {
        aboveNoteEvent.AddEvent(trackEvent);
    }
    public void AddSpeedEvent(KoreographyEvent trackEvent)
    {
        speedEvent.AddEvent(trackEvent);
    }

}
