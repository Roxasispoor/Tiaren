using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public interface ICustomMessageTarget : IEventSystemHandler
{
    // functions that can be called via the messaging system
    void CallForPreview();
}
