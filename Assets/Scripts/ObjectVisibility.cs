using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectVisibility : MonoBehaviour
{
    [SerializeField] private List<Renderer> ignoreRendererList;

    private List<Renderer> _rendererList = new List<Renderer>();

    private void Awake()
    {
        var rendererArray = GetComponentsInChildren<Renderer>(true);
        
        _rendererList.AddRange(rendererArray);
        
        _rendererList.Add(GetComponent<Renderer>());
        
    }


    public void Show()
    {
        foreach (Renderer renderer in _rendererList)
        {
            if (ignoreRendererList.Contains(renderer))
            {
                continue;
            }
            renderer.enabled = true;
        }
    }

    public void Hide()
    {
        foreach (Renderer renderer in _rendererList)
        {
            if (ignoreRendererList.Contains(renderer))
            {
                continue;
            }
            renderer.enabled = false;
        }
    }

}