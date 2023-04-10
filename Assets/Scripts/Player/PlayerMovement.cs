using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : AppearingObject
{
    [SerializeField] CharacterController controller;
    [SerializeField] float speed = 35;
    [SerializeField] float rotationSpeed = 200;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 dir;

    private void Awake()
    {
        LevelManager.OnLevelUpdate += RecreateSelf;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void RecreateSelf(float height)
    {
        _Mesh.localScale = Vector3.zero;
        transform.SetPositionAndRotation(new(initialPosition.x, initialPosition.y + height, initialPosition.z), initialRotation);

        gameObject.SetActive(false);
    }

    private void Start()
    {
        rotationSpeed *= 10;
    }
    void Update()
    {
        if (GameManager.gameStatus != GameManager.GameStatus.GameRunning) return;

        dir = RetrieveInputs();

        MoveAndRotate(dir);
    }

    private Vector3 RetrieveInputs()
    {
        return new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void MoveAndRotate(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.LookRotation(dir),
                rotationSpeed * Time.deltaTime);
        }

        controller.Move(speed * Time.deltaTime * dir);
    }

    private void OnDestroy()
    {
        LevelManager.OnLevelUpdate -= RecreateSelf;
    }

}
