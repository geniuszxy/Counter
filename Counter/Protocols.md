# Login

## Login without password and encryption

Nothing to do, just start quering.

## Login with password

Query

Field           | Type   | Comment
---             | ---    | ---
Method          | byte   | `0`
Password Length | ushort |
Password        | string |

Return

Field       | Type    | Comment
---         | ---     | ---
Error Code  | byte    | [See Error](#error)

## Login with encryption

TODO

# Query

## Query Next Number

Query

Field      | Type   | Comment
---        | ---    | ---
Method     | byte   | `1`
Counter ID | ushort | 0-based

Return

Field       | Type    | Comment
---         | ---     | ---
Error Code  | byte    | [See Error](#error)
Next Number | integer | Based on counter size

## Query Numbers

Query

Field        | Type   | Comment
---          | ---    | ---
Method       | byte   | `2`
Counter ID   | ushort | 0-based
Number Count | byte   | Must be greater than 1

Return

Field       | Type    | Comment
---         | ---     | ---
Error Code  | byte    | [See Error](#error)
Next Number | integer | Based on counter size

## Query Counter Status

Query

Field        | Type   | Comment
---          | ---    | ---
Method       | byte   | `3`
Counter ID   | ushort | 0-based

Return

Field        | Type    | Comment
---          | ---     | ---
Error Code   | byte    | [See Error](#error)
Counter Size | byte    | Available values are 4, 8, 16
Next Number  | integer | Based on counter size

# Administrate

## Add Counter

Query

Field        | Type   | Comment
---          | ---    | ---
Method       | byte   | `4`
Counter ID   | ushort | 0-based
Counter Size | byte   | Available values are 4, 8, 16

Return

Field        | Type    | Comment
---          | ---     | ---
Error Code   | byte    | [See Error](#error)

## Delete Counter

Query

Field        | Type   | Comment
---          | ---    | ---
Method       | byte   | `5`
Counter ID   | ushort | 0-based

Return

Field        | Type    | Comment
---          | ---     | ---
Error Code   | byte    | [See Error](#error)

## Set Counter

Query

Field        | Type    | Comment
---          | ---     | ---
Method       | byte    | `6`
Counter ID   | ushort  | 0-based
Counter Size | byte    | Available values are 4, 8, 16
Next Number  | integer | Based on counter size

Return

Field        | Type    | Comment
---          | ---     | ---
Error Code   | byte    | [See Error](#error)

## Add User

TODO

## Delete User

TODO

# Error

Return data only contains an error code if not success.

Error Code |  L  |  Q  |  A  | Comment
---        | --- | --- | --- | ---
0          |  +  |  +  |  +  | Success
1          |     |  +  |  +  | Wrong counter ID
2          |     |  +  |     | Reach the largest counter number
3          |  +  |  +  |  +  | Authentication failed
4          |     |     |  +  | Exist counter ID
5          |     |     |  +  | Wrong counter size
6          |     |     |  +  | Permission denied
7          |  +  |  +  |  +  | Wrong input