using UnityEngine;

public class Player5 : MainPlayer
{
    [SerializeField] private Sprite smallSprite;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite fatSprite;
    [SerializeField] private AudioClip eat;

    protected override void OnDisable()
    {
        base.OnDisable();
        transform.localScale = new Vector2(1.25f,1.25f);
    }

    public override void TakeDamage(float damage = 10)
    {
        ChangeScale(.25f);
        base.TakeDamage(damage);
    }

    protected override void IncreaseHealth()
    {
        base.IncreaseHealth();
        ChangeScale(-.5f);
    }
    
    void ChangeScale(float scale)
    {
        transform.localScale = new Vector2(transform.localScale.x+scale, 
            transform.localScale.y+scale);
        PlaySound(eat);
        Sr.sprite = ScaleSprite();
    }

    Sprite ScaleSprite()
    {
        switch (transform.localScale.x)
        {
            case < 5:
                return smallSprite;
            case <= 10:
                return normalSprite;
            case > 10:
                return fatSprite;
        }
        return null;
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);
        if (col.gameObject.CompareTag("Wall"))
        {
            health = 0;
            TakeDamage();
        }
    }
}
