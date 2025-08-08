using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration component describing which bird types are available in a scene.
/// Attach this to a GameObject in the scene and populate the list in the editor
/// to restrict the birds that the <see cref="BirdLauncher"/> can spawn.
/// </summary>
public class StageBirdConfig : MonoBehaviour
{
    /// <summary>
    /// List of bird types that are allowed for this stage. When empty, all
    /// birds are considered available.
    /// </summary>
    public List<BirdType.Type> allowedBirdTypes = new List<BirdType.Type>();
}

