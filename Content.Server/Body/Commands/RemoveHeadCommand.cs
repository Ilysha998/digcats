using System.Linq;
using Content.Server.Administration;
using Content.Server.Body.Systems;
using Content.Shared.Administration;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Random;

namespace Content.Server.Body.Commands
{
    [AdminCommand(AdminFlags.Fun)]
    sealed class RemoveHeadCommand : IConsoleCommand
    {
        public string Command => "removehead";
        public string Description => "Removes a head from your entity.";
        public string Help => $"Usage: {Command}";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var player = shell.Player as IPlayerSession;
            if (player == null)
            {
                shell.WriteLine("Only a player can run this command.");
                return;
            }

            if (player.AttachedEntity == null)
            {
                shell.WriteLine("You have no entity.");
                return;
            }

            var entityManager = IoCManager.Resolve<IEntityManager>();
            if (!entityManager.TryGetComponent(player.AttachedEntity, out BodyComponent? body))
            {
                var random = IoCManager.Resolve<IRobustRandom>();
                var text = $"You have no body{(random.Prob(0.2f) ? " and you must scream." : ".")}";

                shell.WriteLine(text);
                return;
            }

            var bodySystem = entityManager.System<BodySystem>();
            var head = bodySystem.GetBodyChildrenOfType(player.AttachedEntity, BodyPartType.Head, body).FirstOrDefault();

            if (head == default)
            {
                shell.WriteLine("You have no head.");
            }
            else
            {
                bodySystem.DropPart(head.Id, head.Component);
            }
        }
    }
}
