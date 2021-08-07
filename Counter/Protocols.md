# Login

## Login without password and encryption

Nothing to do, just start query.

## Login with password

Query

Field           | Type   | Comment
---             | ---    | ---
Method          | byte   | = 0
Password Length | ushort |
Password        | string |

Return

Field       | Type    | Comment
---         | ---     | ---
Error Code  | byte    | [See Error](#login-1)

## Login with encryption

Query

Field           | Type   | Comment
---             | ---    | ---
Method          | byte   | = 0
Password Length | ushort |
Password        | string | TODO

Return

Field       | Type    | Comment
---         | ---     | ---
Error Code  | byte    | [See Error](#login-1)
TODO

# Query

## Query Next Number

Query

Field      | Type   | Comment
---        | ---    | ---
Method     | byte   | = 1
Counter ID | ushort | 0-based

Return

Field       | Type    | Comment
---         | ---     | ---
Error Code  | byte    | [See Error](#query-1)
Next Number | integer | Based on Counter Size

## Query Numbers

Query

Field        | Type   | Comment
---          | ---    | ---
Method       | byte   | = 2
Counter ID   | ushort | 0-based
Number Count | byte   | Must be greater than 1

Return

Field       | Type    | Comment
---         | ---     | ---
Error Code  | byte    | [See Error](#query-1)
Next Number | integer | Based on Counter Size

## Query Counter Status

Query

Field        | Type   | Comment
---          | ---    | ---
Method       | byte   | = 3
Counter ID   | ushort | 0-based

Return

Field        | Type    | Comment
---          | ---     | ---
Error Code   | byte    | [See Error](#query-1)
Counter Size | byte    | Available values are 4, 8, 16
Next Number  | integer | Based on Counter Size

# Administrate

## Add Counter

## Delete Counter

## Set Counter

## Add User

## Delete User

# Error

## Login

Error Code | Comment
---        | ---
0          | Success
3          | Authentication failed

## Query

Error Code | Comment
---        | ---
0          | Success
1          | Wrong counter ID
2          | Reach the largest counter number
3          | Authentication failed

## Administrate