using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Data;
using UnityEngine;

public class ConsoleInitialzer : MonoBehaviour
{
    private void Awake()
    {
        DataManager.Instance.Init();
    }
}
