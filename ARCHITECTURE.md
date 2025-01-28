# NATS-Nexus Showcase Project Architecture 🏗️✨

This document outlines the architecture for the **NATS-Nexus Showcase Project**, leveraging NATS services like KV, JetStream, and WebSocket for dynamic publisher and consumer scaling.

---

## **Project Architecture Overview** 🏗️✨

### **1. Minimal API (WebAPI Layer)**
- **Role**: Acts as the central hub for managing configurations and monitoring the system.
- **Responsibilities**:
  - Expose endpoints to view and modify publisher rate and consumer count.
  - Provide health checks and diagnostics.
  - Handle administrative tasks like resetting KV buckets or reinitializing states.

---

### **2. Infrastructure Layer**
- **Role**: Handles interaction with NATS KV and JetStream for persistence and messaging.
- **Responsibilities**:
  - Manage connection to the NATS server.
  - Provide APIs to read/write values in KV buckets.
  - Trigger events for changes in the `publisher_rate` and `consumer_count` KV buckets.
- **Key Components**:
  - **KVManager**: Monitors changes in KV buckets.
  - **JetStreamManager**: Manages subjects, streams, and subscriptions.

---

### **3. Publisher Project**
- **Role**: Publishes messages to a specific subject based on the rate defined in the `publisher_rate` KV bucket.
- **Responsibilities**:
  - Listen for updates to the `publisher_rate` KV bucket.
  - Dynamically adjust the publication rate.
  - Publish messages to the configured NATS subject.
- **Key Components**:
  - **PublisherService**: Handles the publishing logic.
  - **RateUpdater**: Monitors changes in the `publisher_rate` KV bucket and adjusts the publishing interval.

---

### **4. Consumer Project**
- **Role**: Consumes messages from a specific subject, scaling dynamically based on the `consumer_count` KV bucket.
- **Responsibilities**:
  - Listen for updates to the `consumer_count` KV bucket.
  - Dynamically add or remove consumer instances.
  - Process messages from the configured NATS subject.
- **Key Components**:
  - **ConsumerService**: Handles the consumption logic.
  - **ConsumerScaler**: Manages the creation or removal of consumer instances based on the `consumer_count` KV bucket.

---

## **Flow of Execution** 📈💡

1. **Startup**:
   - The WebAPI initializes and sets up KV buckets (`publisher_rate` and `consumer_count`) with default values.
   - Publisher and Consumer projects start and connect to the NATS server.

2. **Real-Time Monitoring**:
   - The **Infrastructure Layer** continuously monitors KV bucket changes.
   - When a change is detected, appropriate events are triggered for publishers or consumers.

3. **Dynamic Adjustments**:
   - The **Publisher Project** adjusts its publishing rate based on changes to the `publisher_rate` bucket.
   - The **Consumer Project** scales the number of active consumers based on changes to the `consumer_count` bucket.

4. **Steady-State Operation**:
   - Publisher and Consumer projects operate independently, interacting with NATS JetStream and WebSocket as required.

---

