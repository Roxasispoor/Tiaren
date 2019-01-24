using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : ObjectOnBloc {

    private void Awake()
    {
        base.Awake();
        isPickable=true;
        isPicked=false;
        dropOnDeathPicker=true;
        dropOnDeathBlocUnder=true;
    }
}
