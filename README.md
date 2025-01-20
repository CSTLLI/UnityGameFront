# Unity Game Project - Player Statistics & Reviews

## Description
Application de jeu Unity en 3 scènes permettant la gestion des utilisateurs, l'affichage des statistiques des joueurs et la soumission d'avis. L'application intègre une base de données MySQL et un webservice PHP pour la gestion des données.

## Liens utiles
- Backend Repository: [Repository du backend](https://github.com/CSTLLI/unity-app-backend)
- Maquette Figma: [Maquette utilisée ](https://www.figma.com/design/focMglPILeQpcCTwM2TtkO/Space-Operators?node-id=0-1&node-type=canvas&t=splO4TlCDH9JKZJT-0)

## Fonctionnalités principales

### Scène 1 : Connexion Utilisateur
- Interface de connexion avec Canvas/UI Tools
- Formulaire de saisie (nom d'utilisateur et mot de passe)
- Cryptage du mot de passe
- Authentification via requêtes HTTP vers le webservice PHP
- Vérification des credentials et redirection vers la scène 2 si succès

### Scène 2 : Statistiques Joueur
- Tableau DataGrid affichant les statistiques des joueurs
- Récupération des données depuis la base MySQL via le webservice
- Affichage dynamique des informations relatives aux joueurs

### Scène 3 : Zone de Commentaires
- Interface de saisie des avis
- Système d'envoi des commentaires vers la base de données
- Utilisation des requêtes POST/GET avec UnityWebRequest

## Structure de la Base de Données

### Tables principales
- Users (authentification)
- PlayerStats (statistiques des joueurs)
- Comments (avis des utilisateurs)

## Technologies Utilisées
- Unity (interface et logique client)
- PHP (webservice backend)
- MySQL (base de données)
- UnityWebRequest (communication HTTP)

## Installation et Configuration

1. Cloner le repository
2. Configurer la base de données MySQL avec les scripts fournis (fichier de la database dans le repo backend) 
3. Déployer le webservice PHP
4. Configurer les endpoints dans Unity

## Endpoints API

### Authentification
```
// Inscription
POST /api/auth/register
Body: {
    "username": string,
    "password": string
}
Response: {
    "success": boolean,
    "message": string,
    "userId": number
}
```

```
// Connexion
POST /api/auth/login
Body: {
    "username": string,
    "password": string
}
Response: {
    "success": boolean,
    "user": {
        "id": number,
        "username": string
    }
}
```

### Statistiques Joueur
```
GET /api/players/stats
Response: {
    "stats": [
        {
            "playerName": string,
            "gamesPlayed": number,
            "gamesWon": number,
            "gamesLost": number,
            "score": number
        }
    ]
}
```

### Commentaires
```
POST /api/feedback
Body: {
    "playerId": number,
    "comment": string
}
Response: {
    "success": boolean,
    "feedbackId": number
}
```

## Sécurité
- Cryptage des mots de passe
- Validation des données côté serveur
- Gestion des sessions utilisateur
