using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("GroundChecking")]
    [SerializeField] private bool BirdBodyIsGround;
    [SerializeField] private bool BirdWingIsGround;
    [SerializeField] private bool BirdLegIsGround;

    [Header("Radius")]
    [SerializeField] private float radiusForBody = 1.5f;
    [SerializeField] private float radiusForWingAndLeg = 1f;

    [Header("All parametrs")]
    public GameObject Wing;
    private bool Getting;

    public GameObject Leg;

    public float speed = 10;
    public float jump = 8;
    public float moveInput = 1;
    public float forcepower = 20;
    public float time = 0.5f;
    private int TimeValue = 2;

    public LayerMask Ground;

    [SerializeField] private bool Movement = true;

    private Animator animator;
    private Rigidbody2D rigidbody;

    private PhotonView view;

    [SerializeField] private string hashtableName = null;

    private bool CanPlay = false;

    private void Start()
    {
        Player[] players = PhotonNetwork.PlayerList;
        if (players.Length <= 1) hashtableName = "PlayerPos1"; else hashtableName = "PlayerPos2";
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        if(CanPlay == false)
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["CanStart"] == true) CanPlay = true;
    }

    void Update()
    {
        if (!view.IsMine) return;

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() { { hashtableName, gameObject.transform.position.x } });

        if (CanPlay == true)
        {
            AnimationChanging();
            WingGround();
            BodyGround();

            BirdLegIsGround = Physics2D.OverlapCircle(Leg.transform.position, radiusForWingAndLeg, Ground);

            if (Movement == true)
            {
                moveInput = Input.GetAxis("Horizontal");
                rigidbody.velocity = new Vector2(moveInput * speed, rigidbody.velocity.y);

                if (time <= 0)
                {
                    TimeValue = 2;
                    time = 0.5f;
                }

                if (TimeValue == 1)
                {
                    time -= Time.deltaTime;
                    if (time <= 0.25f && BirdLegIsGround == true)
                    {
                        rigidbody.velocity = Vector2.up * jump;
                    }
                }
                if (Input.GetKeyDown(KeyCode.W)) TimeValue = 1;

                if (Input.GetKey(KeyCode.A)) rigidbody.AddForce(Vector2.left * forcepower);
                if (Input.GetKey(KeyCode.D)) rigidbody.AddForce(Vector2.right * forcepower);
            }
        }
    }

    void AnimationChanging() => view.RPC("RPC_Anim", RpcTarget.AllBuffered);

    void BodyGround()
    {
        BirdBodyIsGround = Physics2D.OverlapCircle(this.gameObject.transform.position, radiusForBody, Ground);
        if (BirdBodyIsGround == true && Movement == true)
        {
            rigidbody.velocity = Vector2.up * 5;
            rigidbody.angularVelocity = rigidbody.angularVelocity + Random.Range(-5, 5);
        }
    }

    void WingGround()
    {
        BirdWingIsGround = Physics2D.OverlapCircle(Wing.transform.position, radiusForWingAndLeg, Ground);

        if (Input.GetKeyDown(KeyCode.R))
        {
            Getting = !Getting;
            if (Getting == true && BirdWingIsGround == true)
            {
                Movement = false;
            }
            if (Getting == false)
            {
                Movement = true;
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {

    }

    [PunRPC]
    private void RPC_Anim()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25f && animator.GetBool("Jump") == false && animator.GetBool("Catch") == true)
            animator.SetBool("Jump", true);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f && animator.GetBool("Catch") == false && animator.GetBool("Jump") == true)
            animator.SetBool("Catch", true);

        if (Input.GetKeyDown(KeyCode.W)) animator.SetBool("Jump", false);

        if (Input.GetKeyDown(KeyCode.R)) animator.SetBool("Catch", false);
    }
}
