using Shared.Game;

namespace WorldServer.Game.Map
{
    public class SearchCheckRange : SearchCheck
    {
        private readonly WorldPosition position;
        private readonly float radius;

        public SearchCheckRange(WorldPosition position, float radius)
        {
            this.position = position;
            this.radius   = radius;
        }

        public override bool CheckActor(Actor actor)
        {
            return position.InRadius(actor.Position, radius);
        }
    }
}
