using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

#region GameManager Events
public class GameMenuEvent : SDD.Events.Event
{
}
public class GamePlayEvent : SDD.Events.Event
{
}
public class GamePauseEvent : SDD.Events.Event
{
}
public class GameResumeEvent : SDD.Events.Event
{
}
public class GameOverEvent : SDD.Events.Event
{
}
public class GameVictoryEvent : SDD.Events.Event
{
}
public class CreditsEvent : SDD.Events.Event
{
}

public class ControlsEvent : SDD.Events.Event
{
}

public class GameStatisticsChangedEvent : SDD.Events.Event
{
	public float eBestScore { get; set; }
	public float eScore { get; set; }
	public int eNLives { get; set; }
}

public class GameLevelEvent : SDD.Events.Event
{
	public float levelCoef {get; set;}
}
#endregion

#region MenuManager Events
public class EscapeButtonClickedEvent : SDD.Events.Event
{
}
public class PlayButtonClickedEvent : SDD.Events.Event
{
	public bool fromMenu { get; set; }
	public int levelIndex { get; set; }
}
public class ResumeButtonClickedEvent : SDD.Events.Event
{
}
public class MainMenuButtonClickedEvent : SDD.Events.Event
{
}

public class ControlsButtonClickedEvent : SDD.Events.Event
{
}

public class CreditsButtonClickedEvent : SDD.Events.Event
{
}

public class QuitButtonClickedEvent : SDD.Events.Event
{ }
#endregion

#region Game Event
public class NewEventEvent : SDD.Events.Event
{
	public string EventName { get; set; }
}

public class EventCompletedEvent : SDD.Events.Event
{
	public string EventName { get; set; }
}

public class SetStateGameoverEvent : SDD.Events.Event
{

}

public class SetStateVictoryEvent : SDD.Events.Event
{

}

public class ResetMapEvent : SDD.Events.Event
{

}
#endregion
