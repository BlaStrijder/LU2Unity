using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GetEnvironmentsResponseDto
{
        public string id;
        public string environmentName;
        public string ownerUserName;
        public int maxLength;
        public int maxHeight;
}

public class EnvironmentListWrapper
{
    public List<GetEnvironmentsResponseDto> environments;
}
