using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GetCurrentEnvironmentResponseDto
{
    public string id;
    public string environmentName;
    public string ownerUserName;
    public int currentMaxLength;
    public int currentMaxHeight;
    
}
