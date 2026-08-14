using System.Collections.Generic;
using UnityEngine;

public interface IUpdatable
{
    void UpdateLogic(float deltaTime);
}

public class ModelsUpdater : MonoBehaviour
{
    private readonly List<IUpdatable> _updatables = new(128);

    private void Update()
    {
        float dt = Time.deltaTime;
        for (int i = 0; i < _updatables.Count; i++)
        {
            _updatables[i].UpdateLogic(dt);
        }
    }

    public void Register(IUpdatable model) => _updatables.Add(model);

    public void Unregister(IUpdatable model)
    {
        int index = _updatables.IndexOf(model);
        if (index < 0) return;

        int last = _updatables.Count - 1;
        _updatables[index] = _updatables[last];
        _updatables.RemoveAt(last);
    }

    public void UnregisterAll() => _updatables.Clear();
}