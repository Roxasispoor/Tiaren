Le networkManager ne peut pas être le game si on veut pouvoir utiliser isServer et ne pas laisser la logique du jeu exister coté client par exemple
NetworkServer n'existe que coté server; Si on en a besoin pour retrieve un objet utiliser plutot ClientScene
Pour spawn un machin il faut l'ajouter au networkmanager pour qu'il puisse être répliqué coté client et appeller une méthode statique NetworkServer.Spawn après avoir instancié l'objet
Ne pas oublier de mettre un network transform. Il n'est pas forcément important de transférer beaucoup d'info au client,
n'envoyons que ce qui est nécessaire pour calculer le graphique. Exemple: la couleur des blocs.
A l'inverse ne pas faire de calculs inutiles au serveur? =>Jump.
Les personnages devraient avoir une autorité locale pour pouvoir dire...
Lorsqu'on utilise joueur.Rpc, seul le player concerné recoit l'info, mais sur tous les clients.
Si on envoie un rpc sur un objet dupliqué par tout le monde, ex: GameManager tous les clients font l'action.
Les joueurs adverses sont également présent sur les deux clients, il est donc de bon ton de vérifier quand on le veut qu'il s'agit bien du local player
Pour certaine Reasons (Tm) envoyé des commandes depuis les GUI marche mal, passer par une fonction locale intermédiaire au niveau du player script(required? dunno) marche bien 