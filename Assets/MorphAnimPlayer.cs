using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to objects that we want to morph animate.
/// </summary>
public class MorphAnimPlayer : MonoBehaviour
{
    private MeshFilter meshFilter;
    private List<Datatypes.MorphTarget> morphTargets;

    public void Set(List<Datatypes.MorphTarget> morphTargets, MeshFilter meshFilter)
    {
        this.morphTargets = morphTargets;
        this.meshFilter = meshFilter;
    }

    public void PlayMorph()
    {
        if (morphTargets == null || morphTargets.Count == 0)
        {
            Debug.Log("No morph animations to play.");
            return;
        }

        StartCoroutine(PlayMorphIE());
    }

    private IEnumerator PlayMorphIE()
    {
        int morphIndex = 0;

        Datatypes.MorphTarget currentMorphTarget;

        while (this != null)
        {
            currentMorphTarget = morphTargets[morphIndex];
            Vector3[] startVertexPositions = this.meshFilter.mesh.vertices;

            float lastStartedMorphTime = Time.time;
            // The time point when it should switch to the next morph target.
            float nextIntervalTime = lastStartedMorphTime + currentMorphTarget.interval;

            while (nextIntervalTime > Time.time)
            {
                // All the vertex position that the morph will get this frame.
                List<Vector3> newFrameVertexPositionsList = new List<Vector3>();
                for (int i = 0; i < this.meshFilter.mesh.vertices.Length; i++)
                {
                    Vector3 newPos = Vector3.Lerp(startVertexPositions[i], currentMorphTarget.positions[i], (Time.time - lastStartedMorphTime) / currentMorphTarget.interval);
                    newFrameVertexPositionsList.Add(newPos);
                }

                // Set the new positions of this frame.
                this.meshFilter.mesh.SetVertices(newFrameVertexPositionsList);

                yield return new WaitForEndOfFrame();
            }

            // Iterate the index.
            morphIndex++;
            if (morphIndex >= morphTargets.Count)
            {
                morphIndex = 0;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
