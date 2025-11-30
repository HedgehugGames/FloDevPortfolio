using UnityEngine;

public abstract class BaseState<T> where T : MonoBehaviour
{
    protected T Owner;
    protected Animator Animator;
    protected const float CrossFadeDuration = 0.1f;

    protected BaseState(T owner)
    {
        this.Owner = owner;
        this.Animator = (owner as MonoBehaviour).GetComponent<Animator>();
    }
    
    public virtual void OnEnter() { }
    public virtual void Update() { }
    public virtual void OnExit() { }
   
    
}