using _Scripts.Game;
using Zenject;
using UnityEngine;

public class LineHandleInstaller : MonoInstaller
{
    [SerializeField] private LineHandleController lineHandlePrefab;
    [SerializeField] private LineSegment lineSegmentPrefab;

    public override void InstallBindings()
    {
        Container.BindMemoryPool<LineSegment, LineSegment.Pool>()
            .WithInitialSize(10)
            .FromComponentInNewPrefab(lineSegmentPrefab)
            .UnderTransformGroup("LineSegments");
        
        Container.BindMemoryPool<LineHandleController, LineHandleController.Pool>()
            .FromComponentInNewPrefab(lineHandlePrefab)
            .UnderTransformGroup("LineHandles");
    }
}