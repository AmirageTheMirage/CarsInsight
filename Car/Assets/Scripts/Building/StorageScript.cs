using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StorageScript : MonoBehaviour
{
    public ParticleSystem StorageParticles;
    public float StorageValue;
    public float StorageMax = 1000f; //it can hold 1000 "Elements"
    private float ParticleMax;
    public bool Fill = false;
    public bool ExportingEnabled = true;
    public EditProperties EditScript;
    //public TMP_Text TextView;
    void Start()
    {
        //StorageValue = StorageMax;
        ParticleMax = StorageParticles.emission.rateOverTime.constant;
    }

    void FixedUpdate()
    {
        if (EditScript.StorTransfer != 0f)
        {
            Debug.Log("Transfer");
            StorageValue += EditScript.StorTransfer;
            EditScript.StorTransfer = 0f;
        }
        ExportingEnabled = EditScript.StatusActive;
        if (StorageValue > 1f)
        {
        EditScript.Storage_StoredAmount = StorageValue;

        } else
        {
            EditScript.Storage_StoredAmount = 0f; //This is to prevent one Block to stay (because rounding-mistake, 0.6 Blocks will be displayed as 1, but that 0.6 Block can't be used.)
        }
        EditScript.Storage_MaxStoredAmount = StorageMax;
        if (StorageValue < 0f)
        {
            StorageValue = 0f;
        }
        var emissionModule = StorageParticles.emission;
        float NewEmission = StorageValue / StorageMax * ParticleMax; //Percentage of "Full" value of Particles
        emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(NewEmission);
        if (Fill)
        {
            Fill = false;
            StorageValue = StorageMax;
        }

        if (StorageValue > StorageMax)
        {
            StorageValue = StorageMax;
        }
    }
}
