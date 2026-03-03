using System.Collections.Generic;

public class RaycastTargetManager : Singleton<RaycastTargetManager>
{
    private Dictionary<int, RaycastTarget> activeRaycastTargets = new();

    private int currentId = 0;

    public void AddRaycastTarget(RaycastTarget raycasTarget)
    {
        raycasTarget.SetId(currentId);

        activeRaycastTargets.Add(currentId, raycasTarget);

        currentId++;
    }

    public void RemoveRaycasTarget(int id)
    { activeRaycastTargets.Remove(id); }

    public RaycastTarget GetRaycastTargetById(int id) 
    {  return activeRaycastTargets[id]; }
}
