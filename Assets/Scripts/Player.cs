using MLAPI;

public class Player : NetworkBehaviour
{
    public Hand hand;

    private void Start()
    {
        hand = GetComponent<Hand>();
    }
}