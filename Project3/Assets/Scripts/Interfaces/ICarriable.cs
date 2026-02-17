using UnityEngine;

public interface ICarriable 
{
    CarryWeightProfile CarryWeightProfile { get; }
    Rigidbody Rigidbody { get; }
    void EnterCarriedState(Transform carryPoint);
    void ExitCarriedState(Vector3 throwForce);

}
