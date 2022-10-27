using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallController : MonoBehaviour
{
    public Rigidbody rigidbody;
    public float speed;

    private bool _isMoving;
    private Vector3 _travelDirection;
    private Vector3 _nextCollisionPosition;

    public int minSwipeRecognition = 500;
    private Vector2 _swipePositionLastFrame;
    private Vector2 _swipePositionCurrentFrame;
    private Vector2 _currentSwipe;

    private Color _solveColour;
    private AudioSource _audioSource;
    public AudioClip audioClip;
    public ParticleSystem entranceParticle;

    // Start is called before the first frame update
    void Start()
    {
        _solveColour = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = _solveColour;
        _audioSource = GetComponent<AudioSource>();
        entranceParticle.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            entranceParticle.Stop();
            rigidbody.velocity = speed * _travelDirection;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f);

        int i = 0;

        while (i < hitColliders.Length)
        {
            GroundPiece ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            if (ground && !ground.isColoured)
            {
                ground.ChangeColour(_solveColour);
            }

            i++;
        }

        if (_nextCollisionPosition != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, _nextCollisionPosition) < 1)
            {
                _isMoving = false;
                _travelDirection = Vector3.zero;
            }
        }

        if (_isMoving)
            return;

        if (Input.GetMouseButton(0))
        {
            _swipePositionCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (_swipePositionLastFrame != Vector2.zero)
            {
                _currentSwipe = _swipePositionCurrentFrame - _swipePositionLastFrame;

                if (_currentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                _currentSwipe.Normalize();

                //UpOrDown
                if (_currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f)
                {
                    SetDestination(_currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                }

                //LeftOrRight
                if (_currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
                {
                    SetDestination(_currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }
            }

            _swipePositionLastFrame = _swipePositionCurrentFrame;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _swipePositionLastFrame = Vector2.zero;
            _currentSwipe = Vector2.zero;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Contains("WallPiece"))
        {
            _audioSource.PlayOneShot(audioClip);
        }
    }

    private void SetDestination(Vector3 direction)
    {
        _travelDirection = direction;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            _nextCollisionPosition = hit.point;
        }

        _isMoving = true;
    }
}