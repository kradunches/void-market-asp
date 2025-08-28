using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserService.Models;

public class User
{
    /// <summary>
    /// GUID id
    /// </summary>
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("email")]
    public string Email { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

// todo: должен быть такой эндпоинт (сделать это как dto):
// id - mongo object id
// get отдельного юзера по id не делать
// {
//     "data": [
//     {
//         "id": "6852f891466a198e14fe93a6",
//         "email": "Annie5@hotmail.com",
//         "name": "Annette Bruen"
//     },
//     {
//         "id": "6852f891466a198e14fe93a7",
//         "email": "Kurt.Haley87@yahoo.com",
//         "name": "Roy Runolfsson"
//     },
//     {
//         "id": "6852f892466a198e14fe93a8",
//         "email": "Margaretta_Botsford39@yahoo.com",
//         "name": "Judith Bauch"
//     },
//     {
//         "id": "6852f892466a198e14fe93a9",
//         "email": "Dan45@hotmail.com",
//         "name": "Rickey Lockman"
//     },
//     {
//         "id": "6852f892466a198e14fe93aa",
//         "email": "Geovany32@gmail.com",
//         "name": "Brittany Cummerata"
//     },
//     {
//         "id": "6852f892466a198e14fe93ab",
//         "email": "Garland69@yahoo.com",
//         "name": "Meredith Stanton"
//     },
//     {
//         "id": "6852f892466a198e14fe93ac",
//         "email": "Roselyn49@gmail.com",
//         "name": "Mildred Rosenbaum IV"
//     },
//     {
//         "id": "6852f892466a198e14fe93ad",
//         "email": "Viviane35@yahoo.com",
//         "name": "Ms. Lori Farrell"
//     },
//     {
//         "id": "6852f892466a198e14fe93ae",
//         "email": "Matilde74@hotmail.com",
//         "name": "Mona Bednar Jr."
//     },
//     {
//         "id": "6852f892466a198e14fe93af",
//         "email": "Callie90@gmail.com",
//         "name": "Ernest Moen"
//     }
//     ],
//     "meta": {
//         "total": 1000,
//         "limit": 10,
//         "offset": 0
//     }
// }