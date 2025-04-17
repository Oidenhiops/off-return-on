using System;
using UnityEngine;

public class ManagementCharacter : MonoBehaviour
{
    public Character character;
    void OnValidate()
    {
        character.owner = this;
    }
    void Start()
    {
        if (TryGetComponent<IMovement>(out IMovement movement)) character.movementCs = movement;
        if (TryGetComponent<IDirection>(out IDirection direction)) character.directionCs = direction;
        if (TryGetComponent<IInteract>(out IInteract interact)) character.interactCs = interact;
        _= character.InitializeCharacter();
    }
    void Update()
    {
        if (GameManager.Instance.startGame && character.isActive)
        {
            if (character.movementCs != null) character.movementCs.HandleMove();
            if (character.directionCs != null) character.directionCs.HandleDirection();
            if (character.interactCs != null) character.interactCs.HandleInteract();
        }
    }
    [Serializable] public class Character
    {
        public bool isActive;
        public Rigidbody rb;
        public bool isGrounded => CheckIsGrounded();
        public IMovement movementCs;
        public IDirection directionCs;
        public IInteract interactCs;
        public CollidersInfo collidersInfo = new CollidersInfo();
        bool CheckIsGrounded(){
            return CurrentGrounded() != "";
        }
        public string CurrentGrounded(){            
            Collider[] hit = Physics.OverlapBox
            (
                owner.transform.position + collidersInfo.offsetCheckGround,
                collidersInfo.sizeCheckGround / 2,
                Quaternion.identity,
                collidersInfo.layerCheckGround
            );
            return hit.Length > 0 ? hit[0].gameObject.tag : "";
        }
        public async Awaitable InitializeCharacter(){
            await Awaitable.NextFrameAsync();
            isActive = true;
        }
        [Serializable] public class CollidersInfo
        {
            public bool useGizmos = false;
            public Vector3 offsetCheckGround;
            public Vector3 sizeCheckGround;
            public LayerMask layerCheckGround;
        }
        [NonSerialized] public MonoBehaviour owner;
    }
    void OnDrawGizmos()
    {
        if (!character.collidersInfo.useGizmos) return;

        Gizmos.color = character.isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube
        (
            transform.position + character.collidersInfo.offsetCheckGround,
            character.collidersInfo.sizeCheckGround
        );
    }
    public interface IMovement
    {
        public void HandleMove();
    }
    public interface IDirection{
        public void HandleDirection();
    }

    public interface IInteract
    {
        public void HandleInteract();
    }
}
