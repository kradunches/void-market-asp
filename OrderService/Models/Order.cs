using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Models;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string UserId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }
    public List<Item> Items { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


// todo: сделать пагинацию и такой dto:
// http://localhost:4000/api/orders/all?offset=0&limit=10
// {
//     "orders": [
//         {
//             "id": 1,
//             "user": {
//                 "id": "6852f891466a198e14fe93a6",
//                 "name": "Annette Bruen",
//                 "email": "Annie5@hotmail.com"
//             },
//             "status": "shipped",
//             "total": 1496.29,
//             "items": [
//                 {
//                     "name": "Small Aluminum Cheese",
//                     "quantity": 7,
//                     "unitPrice": 92.2
//                 },
//                 {
//                     "name": "Rustic Granite Pizza",
//                     "quantity": 8,
//                     "unitPrice": 5.75
//                 },
//                 {
//                     "name": "Bespoke Marble Ball",
//                     "quantity": 7,
//                     "unitPrice": 80.55
//                 },
//                 {
//                     "name": "Sleek Bronze Tuna",
//                     "quantity": 7,
//                     "unitPrice": 32.15
//                 },
//                 {
//                     "name": "Ergonomic Marble Ball",
//                     "quantity": 1,
//                     "unitPrice": 15.99
//                 }
//             ],
//             "createdAt": "2025-06-18T17:34:09.873347Z",
//             "updatedAt": "2025-06-18T17:34:09.873347Z"
//         },
//         {
//             "id": 2,
//             "user": {
//                 "id": "6852f891466a198e14fe93a6",
//                 "name": "Annette Bruen",
//                 "email": "Annie5@hotmail.com"
//             },
//             "status": "shipped",
//             "total": 955.25,
//             "items": [
//                 {
//                     "name": "Handcrafted Plastic Cheese",
//                     "quantity": 7,
//                     "unitPrice": 74.8
//                 },
//                 {
//                     "name": "Incredible Bamboo Bike",
//                     "quantity": 3,
//                     "unitPrice": 62.65
//                 },
//                 {
//                     "name": "Ergonomic Steel Bacon",
//                     "quantity": 8,
//                     "unitPrice": 10.55
//                 },
//                 {
//                     "name": "Practical Rubber Mouse",
//                     "quantity": 2,
//                     "unitPrice": 79.65
//                 }
//             ],
//             "createdAt": "2025-06-18T17:34:09.889379Z",
//             "updatedAt": "2025-06-18T17:34:09.889379Z"
//         },
//         {
//             "id": 3,
//             "user": {
//                 "id": "6852f891466a198e14fe93a6",
//                 "name": "Annette Bruen",
//                 "email": "Annie5@hotmail.com"
//             },
//             "status": "pending",
//             "total": 734.1,
//             "items": [
//                 {
//                     "name": "Frozen Cotton Sausages",
//                     "quantity": 5,
//                     "unitPrice": 10.65
//                 },
//                 {
//                     "name": "Luxurious Marble Salad",
//                     "quantity": 9,
//                     "unitPrice": 75.65
//                 }
//             ],
//             "createdAt": "2025-06-18T17:34:09.893471Z",
//             "updatedAt": "2025-06-18T17:34:09.893471Z"
//         },
//         {
//             "id": 4,
//             "user": {
//                 "id": "6852f891466a198e14fe93a6",
//                 "name": "Annette Bruen",
//                 "email": "Annie5@hotmail.com"
//             },
//             "status": "cancelled",
//             "total": 498.79999999999995,
//             "items": [
//                 {
//                     "name": "Ergonomic Gold Towels",
//                     "quantity": 1,
//                     "unitPrice": 68.35
//                 },
//                 {
//                     "name": "Fantastic Marble Cheese",
//                     "quantity": 10,
//                     "unitPrice": 14.7
//                 },
//                 {
//                     "name": "Fresh Gold Chicken",
//                     "quantity": 5,
//                     "unitPrice": 56.69
//                 }
//             ],
//             "createdAt": "2025-06-18T17:34:09.899064Z",
//             "updatedAt": "2025-06-18
// "unitPrice": 60.3
//                 }
//             ],
//             "createdAt": "2025-06-18T17:34:09.903385Z",
//             "updatedAt": "2025-06-18T17:34:09.903385Z"
//         }
//     ],
//     "total": 10000
// }


// todo: это то как должен возвращаться order по конкретному id
// это нужно сделать через api gateway:
// 1) делаем запрос на order service
// 2) получаем из ответа userId
// 3) делаем запрос на user service 
// 4) c user service получаем ответ
// 5) ответ order service + user service складываем в один джейсон, который описан ниже
// 6) из результата убираем userId
// {
// "id": 3,
// "user": {
//     "id": "6852f891466a198e14fe93a6",
//     "name": "Annette Bruen",
//     "email": "Annie5@hotmail.com"
// },
// "status": "pending",
// "total": 734.1,
// "items": [
// {
//     "name": "Frozen Cotton Sausages",
//     "quantity": 5,
//     "unitPrice": 10.65
// },
// {
// "name": "Luxurious Marble Salad",
// "quantity": 9,
// "unitPrice": 75.65
// }
// ],
// "createdAt": "2025-06-18T17:34:09.893471Z",
// "updatedAt": "2025-06-18T17:34:09.893471Z"
// }

// todo: в postgres orders должно быть 2 таблицы:
// items и orders