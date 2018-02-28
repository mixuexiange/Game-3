using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class playerController : MonoBehaviour {
    Vector3 pos;
    public float speed = 1f;

    public Tilemap tilemap;
    public Tile destroy;
    private GameObject changedFloor;
    private Vector2 mousePos;
    private LayerMask allTilesLayer;
    //public Vector3Int cell;

    void Start()
    {
        pos = transform.position; // Take the current position

    }
    void FixedUpdate()
    {
        //====RayCasts====//
        RaycastHit2D hitup = Physics2D.Raycast(transform.position, Vector2.up, 16);
        RaycastHit2D hitdown = Physics2D.Raycast(transform.position, Vector2.down, 16);
        RaycastHit2D hitright = Physics2D.Raycast(transform.position, Vector2.right, 16);
        RaycastHit2D hitleft = Physics2D.Raycast(transform.position, Vector2.left, 16);
        //==Inputs==//
        if (Input.GetKey(KeyCode.A) && transform.position == pos && hitleft.collider.tag != "wall")
        {           //(-1,0)
            pos += Vector3.left * 16;// Add -1 to pos.x
        }
		if (Input.GetKey(KeyCode.D) && transform.position == pos && hitright.collider.tag != "wall")
        {           //(1,0)
            pos += Vector3.right * 16;// Add 1 to pos.x
        }
		if (Input.GetKey(KeyCode.W) && transform.position == pos && hitup.collider.tag != "wall")
        {           //(0,1)
            pos += Vector3.up * 16; // Add 1 to pos.y
        }
		if (Input.GetKey(KeyCode.S) && transform.position == pos && hitdown.collider.tag != "wall")
        {           //(0,-1)
            pos += Vector3.down * 16;// Add -1 to pos.y
        }
        //The Current Position = Move To (the current position to the new position by the speed * Time.DeltaTime)


        transform.position = Vector3.MoveTowards(transform.position, pos, speed);    // Move there


        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(transform.position);
        Vector3Int cell = tilemap.WorldToCell(transform.position);
        //Vector3 cellCenterPos = tilemap.GetCellCenterWorld(cell);

        Tile tile = tilemap.GetTile<Tile>(cell);
        if (tile == destroy)
        {
            tilemap.SetTile(cell, null);
            Debug.Log("sddddddddddddd");
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "bad")
        {
            Debug.Log("why wont you work ;_;");
        }
    }
    private void Update()
    {

    }

}
