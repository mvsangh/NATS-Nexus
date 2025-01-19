# NATS Streaming Showcase Project

## Overview 🎯🎉🚀

This project demonstrates the powerful features of NATS Streaming by integrating various NATS services such as **NATS Key-Value (KV)**, **NATS JetStream**, and **NATS WebSocket**. The primary use case involves dynamically controlling the rate of message publication and the number of consumers subscribing to messages, leveraging real-time updates in NATS KV buckets.

## Features ✨📦🌟

- **Dynamic Publisher Rate Control**: Adjust the message publication rate in real-time by updating the "publisher rate" bucket in NATS KV.
- **Dynamic Consumer Scaling**: Scale the number of message consumers dynamically by modifying the "consumer count" bucket in NATS KV.
- **Event-Driven Architecture**: Handle KV change events to dynamically reconfigure publishers and consumers.
- **Integration with NATS JetStream**: Use JetStream for high-performance messaging and stream persistence.
- **WebSocket Support**: Demonstrates interaction with NATS through WebSocket for modern, lightweight client-server communication.

## Prerequisites 📋🔧✅

- **NATS Server**: Ensure a NATS server is running with **WebSocket** and **JetStream** enabled.
- **.NET 9.0 or later**: Install the latest version of the .NET SDK.

## Project Structure 📂🗂️📁

```
NATS-Streaming-Showcase/
├── README.md
├── .gitignore
└── Streaming.sln
```

## Setup Instructions 🚀📖🛠️

### Step 1: Clone the Repository 🖥️📥📂

```bash
git clone <repository-url>
```

### Step 2: Configure NATS Server 📝🔧⚙️

Update the `appsettings.json` file with the appropriate NATS server connection details:

```json
{
    "ConnectionStrings"{
        "nats":"nats://<ip>:4222"
    }
}
```

### Step 3: Run the Project 🏃‍♂️⚡🚀

1. Build the solution:
   ```bash
   dotnet build
   ```
   
## How It Works 🤔🔍💡

1. **Initialize KV Buckets**: Upon starting the application, two buckets are created in NATS KV:

   - `publisher_rate`: Stores the rate of messages to publish per minute.
   - `consumer_count`: Stores the number of active consumers.

2. **Real-Time Adjustments**:

   - When the value in the `publisher_rate` bucket changes, the publisher adjusts its message publication rate dynamically.
   - When the value in the `consumer_count` bucket changes, the system creates or removes consumers as necessary.

3. **Event-Driven Execution**:

   - The `KVManager` component listens for changes in the KV buckets and triggers the necessary updates to the publisher and consumer components.

## Example Scenarios 💡📈🛠️

1. **Increase Publisher Rate**:

   - Update the `publisher_rate` bucket to `20`.
   - The publisher starts publishing 20 messages per minute.

2. **Scale Consumers**:

   - Update the `consumer_count` bucket to `3`.
   - The system creates additional consumers until 3 are active, all subscribing to the same subject.

## Dependencies 📦🔗📚

- [NATS .NET Client](https://github.com/nats-io/nats.net)
- [NATS JetStream .NET](https://github.com/nats-io/nats.net/tree/main/src/JetStream)

## Contributing 🤝💡✨

Contributions are welcome! Feel free to submit issues or pull requests to improve this project.

## License 📝🔓✅

This project is licensed under the MIT License. See the LICENSE file for details.

---

Enjoy exploring the power of NATS Streaming! 🎉🚀✨

