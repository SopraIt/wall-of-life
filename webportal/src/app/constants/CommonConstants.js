export default {
    dbUrl: 'http://localhost:3000/db',
    dbPeopleUrl: 'http://localhost:3000/people',
    dbPrizeUrl: 'http://localhost:3000/prizes',
    dbStarsUrl: 'http://localhost:3000/stars',
    base64Image: 'http://localhost:8080/save_base64',
    uploadTrophy: 'http://localhost:8080/prize_upload',
    uploadImage: 'http://localhost:8080/upload_image',
    renameImage: 'http://localhost:8080/rename_image',
    homeIcons: [
        {
            img: 'add-user.png',
            label: 'Aggiungi <br />Utente',
            url: '/user'
        },
        {
            img: 'group.png',
            label: 'Cerca <br />Utente',
            url: '/search'
        },
        {
            img: 'trophy.png',
            label: 'Gestione <br />premi',
            url: '/trophy'
        },
    ],
    userImages: [
        {
            name: 'cat',
            url: 'cat.png'
        },
        {
            name: 'pig',
            url: 'pig.png'
        },
        {
            name: 'dog',
            url: 'dog.png'
        },
        {
            name: 'fish',
            url: 'fish.png'
        },
        {
            name: 'owl',
            url: 'owl.png'
        },
        {
            name: 'monkey',
            url: 'monkey.png'
        },
        {
            name: 'lion',
            url: 'lion.png'
        },
        {
            name: 'chicken',
            url: 'chicken.png'
        },
    ],
};
