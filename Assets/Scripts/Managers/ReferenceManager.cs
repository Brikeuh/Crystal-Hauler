using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class PanelReferences
{
    public PanelType _PanelType;
    public GameObject Panel;
    public Image PaneImage;
    public TextMeshProUGUI Txt;
}
public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance;
    public JammoPlayerController PlayerController;
    public List<PanelReferences> PanelReferencesList = new List<PanelReferences>();
    public Transform _Cam;
   

    internal void RegisterPanel(PanelType panelType, GameObject uIPanel)
    {
        PanelReferencesList[(int)panelType].Panel = uIPanel.gameObject;
    }

    
    public PanelReferences GetPanel(PanelType panelType)
    {
        foreach (var v in PanelReferencesList)
        {
            if (v._PanelType == panelType)
                return v;
        }
        return null;
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
