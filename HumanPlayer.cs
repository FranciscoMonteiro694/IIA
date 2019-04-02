using UnityEngine;

public class HumanPlayer : PlayerController {
	
	public override void Start () {
        base.Start();
        interactive = true;
        source = null;
        destination = null;
        doattack = false;
    }

	public override void TurnUpdate ()
	{
    
        if(source != null && destination != null)
        {
            if (doattack)
            {
                GameManager.instance.tileboard[(int)destination.gridPosition.x, (int)destination.gridPosition.y].GetComponent<Renderer>().material.color = Color.red;
                Attack(source, destination);
                GameManager.instance.NextTurn();
                GameManager.instance.tileboard[(int)destination.gridPosition.x, (int)destination.gridPosition.y].GetComponent<Renderer>().material.color = Color.white;
                doattack = false;
                destination = null;
                source = null;
            }
            else {
                base.MoveAnimation();
            }
            
        }
    }


    public void UpdatePlay(Tile tile)
    {
        if(tile.hasUnit() && this.IsMyUnit(tile.inTile))
        {
            if(this.source == null) 
            { 
                this.source = tile;
                tile.SetSelected(true);
                MarkNeighbours(source, true);
            }
            else
            {
                if(this.source == tile)
                {
                   MarkNeighbours(source, false);
                    this.source = null;
                    tile.SetSelected(false);

                }

            }

        } else
        {
            if (!tile.hasUnit() && !tile.HasWall && source != null && source.IsNeighbour(tile))
            {
                destination = tile;
            }
            else
            {
                if (tile.hasUnit() && source != null && !this.IsMyUnit(tile.inTile) && source.inTile.GetAttackable().Contains(tile.inTile))
                {
                    destination = tile;
                    doattack = true;
                }
            }
        }
    }

    public Tile GetSource()
    {
        return source;
    }

}
