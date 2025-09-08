#Document Metadata & Search System Design
#Overview
This document outlines the architecture and design for a system to ingest Chemical Safety Data Sheets (SDS) in PDF format, extract metadata, manage document versions, and enable fast search capabilities. The system is designed to be multi-tenant, support role-based access control (RBAC), and ensure compliance with service-level agreements (SLAs) for ingestion and search performance.

#System Requirements
#Functional Requirements
(1) File Upload: Support PDF uploads up to 25 MB.
Versioning: Version documents based on tenantId, supplier, sku, docType, and version/batch.
Metadata Extraction: Extract and store metadata in JSON format (assumed OCR/ML recognizer is available).
Search Functionality: Enable searches by SKU, supplier, docType, and date ranges. Support queries for the latest version only.
Compression Service: Compress uploaded PDF files to minimize storage requirements.
Decompression Service: Decompress files when downloading to ensure users receive the original format.
Multi-Tenant Architecture: Implement a simple RBAC model (role/tenant).
Retention Policy: Retain the last N versions of documents (configurable) and archive older versions.
Email Worker: Send email notifications for various events.
SMS Worker: Send SMS notifications for various events.
Push Notification Service: Manage push notifications to mobile and web applications.
Dead Letter Queue (DLQ): Implement a DLQ for handling failed processing events to facilitate self-healing.

Non-Functional Requirements
Ingestion SLA: 95th percentile ≤ 5 seconds (excluding long-running OCR).
Search SLA: 95th percentile ≤ 200 milliseconds.
Availability: 99.9% availability for search APIs.
Proposed Architecture
Components
Upload API: Handles file uploads, possibly using presigned URLs or direct POST to blob storage.
Download API: Facilitates retrieval of documents, including decompression.
Compression Service: Compresses uploaded PDFs to reduce storage space.
Decompression Service: Decompresses files when serving downloads.
Retention Worker: Manages retention policies for document versions.
Configuration API: Provides configurable settings (e.g., retention limits).
Archive Worker: Archives older document versions.
OCR/ML Recognizer Service: Extracts metadata from PDFs.
MetaData Worker: Writes extracted metadata to the database.
Load Balancer: Distributes incoming requests across API instances.
API Gateway: Serves as the entry point for Web/Mobile/Integration consumers, ensuring multi-tenant separation.
Email Worker: Sends email notifications for various events.
SMS Worker: Sends SMS notifications for various events.
Push Notification Service: Manages push notifications to mobile and web applications.
Dead Letter Queue (DLQ): Captures failed processing events for manual or automated reprocessing.
Data Stores
Metadata Database: PostgreSQL for structured metadata storage.
ReplicaSets: Implement ReplicaSets for high availability and load balancing.
Sharding: Use sharding to distribute data across multiple database instances based on tenantId or other relevant keys.
Partitioning: Partition tables to optimize performance for large datasets.
Cache: Redis for caching frequently accessed data and implementing idempotency/outbox patterns.
Message Broker: Kafka for event-driven communication between services, including the DLQ mechanism.
Observability
Logging & Monitoring: ELK stack for logs, metrics, and traces; use Datadog or Prometheus for monitoring and alerts.
Security
HSM: Hardware Security Module for key management.
Identity Server: Manages role-based authentication and authorization.
File Validation: Sandbox for scanning and validating uploaded files before processing.
PKI/TLS: Use TLS certificates and mutual TLS for secure communication.
Data Flow
Upload: A user uploads a PDF through the API Gateway, which routes it to the Upload API.
Storage: The file is initially stored in object storage (e.g., AWS S3 or Azure Blob Storage).
Compression: The Compression Service processes the PDF to reduce its size.
Queue: An event is placed in a message queue (Kafka) for processing.
Metadata Extraction: The OCR/ML service processes the compressed PDF, extracting metadata.
Metadata Storage: The metadata is written to PostgreSQL by the MetaData Worker, utilizing sharding and partitioning for optimal performance.
Indexing: The metadata is indexed for fast search capabilities.
Retention Management: The Retention Worker ensures that only the last N versions are retained, while older versions are archived.
Download: When a user requests to download a document, the Decompression Service retrieves the compressed file, decompresses it, and serves the original PDF.
Notifications: Based on events (e.g., upload completion), the Email Worker, SMS Worker, and Push Notification Service send notifications to users.
Error Handling: If any processing fails, the event is sent to the DLQ for later investigation and reprocessing.
Trade-offs
Storage vs. Performance: Using object storage for PDFs allows scalability but may introduce latency in retrieval compared to traditional databases. Compression reduces storage costs but may slightly increase processing time for uploads and downloads.
Complexity vs. Functionality: An event-driven architecture introduces complexity but enhances decoupling and scalability.
Cost vs. Performance: Utilizing managed services for storage and indexing may increase costs but ensures reliability and reduces operational overhead.
Database Sharding and Partitioning: While these techniques improve performance and scalability, they add complexity to data management and queries.
DLQ Implementation: Introducing a DLQ adds resilience to the system by allowing for recovery from processing failures but requires additional monitoring and management.
Architecture Diagram
[System Architecture Diagram]

(Note: Replace with an actual diagram illustrating components, data flow, and interactions.)

Cost and Operability Notes
Cost Considerations
Storage Costs: Assess costs for object storage and database usage.
Compute Costs: Evaluate costs for running services (e.g., API instances, processing services).
Licensing/Management Costs: Consider costs for third-party tools for monitoring and observability.
Operability
Monitoring and Alerts: Implement dashboards for system performance and alerting on service health.
On-call Rotation: Establish an on-call rotation for handling incidents, with clear escalation paths.
Service Level Objectives (SLOs): Define and monitor SLOs for ingestion, search performance, and availability.

# C4 Model Diagrams

## Level 1: System Context
```plantuml
@startuml
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Context.puml

Person(user, "User", "Ops/Quality/Safety user uploading/searching SDS PDFs")

System_Boundary(sds, "SDS Platform") {
  System(sds_sys, "SDS Ingestion & Search System", "Ingest, extract metadata, version, and search SDS PDFs")
}

System_Ext(oidc, "OIDC Provider", "Identity provider for authentication")
System_Ext(suppliers, "Supplier Systems", "External sources providing PDFs (optional)")
System_Ext(notify, "Notification Systems", "Email/SMS/Push (optional)")

Rel(user, sds_sys, "Uploads PDFs; searches documents", "HTTPS/JSON")
Rel(sds_sys, oidc, "AuthN (OIDC/JWT)", "OIDC/OAuth2")
Rel(sds_sys, suppliers, "Pulls/receives PDFs (optional)", "HTTPS/SFTP")
Rel(sds_sys, notify, "Sends notifications (optional)", "HTTPS/SMTP/SMS")

@enduml
```

## Level 2: Containers
```plantuml
@startuml
!include https://raw.githubusercontent.com/plantuml-stdlib/C4-PlantUML/master/C4_Container.puml

Person(user, "User", "Ops/Quality/Safety user")
System_Boundary(sds, "SDS Ingestion & Search System") {
  Container(upload_api, "Upload API", "Go/Java/Node", "Streams uploads (≤25MB) to NFS; idempotency; emits Kafka events; RBAC/OIDC")
  Container(metadata, "Metadata Service", "Go/Java/.NET", "Validates, versions, stores metadata in Postgres; exposes Search API; outbox -> Kafka")
  Container(ocr, "OCR/ML Service", "Python", "Extracts text/entities from PDFs")
  Container(config, "Configuration Service", "Any", "Per-tenant retention N and feature flags")
  Container(retention, "Retention Service", "Any", "Enforces keep-last-N; triggers Archive")
  Container(archive, "Archive Service", "Any", "Moves files to archive namespace; marks archived in Postgres")
  Container(indexer, "Indexer", "Any", "Consumes MetadataCommitted; updates OpenSearch")
  ContainerDb(pg, "Postgres", "RDBMS", "Metadata, versioning, RBAC")
  ContainerDb(os, "OpenSearch", "Search", "Secondary index for fast queries")
  ContainerDb(redis, "Redis", "KV Store", "Idempotency keys; rate limits")
  ContainerDb(nfs, "NFS", "File storage", "PDF binaries (NFS protocol)")
  ContainerQueue(kafka, "Kafka", "Event streaming", "UploadCompleted, MetadataCommitted, Archived")
}

System_Ext(oidc, "OIDC Provider", "AuthN")

Rel(user, upload_api, "Upload PDFs, authenticated", "HTTPS/JWT")
Rel(user, metadata, "Search by SKU/supplier/docType/date", "HTTPS/JWT")

Rel(upload_api, oidc, "Validate tokens", "OIDC/JWT")
Rel(upload_api, redis, "SETNX dedup keys", "TCP")
Rel(upload_api, nfs, "Stream write PDFs", "NFS")
Rel(upload_api, kafka, "Publish UploadCompleted", "Kafka")

Rel(kafka, ocr, "Consume UploadCompleted", "Kafka")
Rel(ocr, nfs, "Read PDFs", "NFS")
Rel(ocr, metadata, "POST extracted JSON", "HTTP")

Rel(metadata, pg, "Upsert docs/versions; store metadata", "SQL/TCP")
Rel(metadata, kafka, "Publish MetadataCommitted (outbox)", "Kafka")
Rel(indexer, kafka, "Consume MetadataCommitted", "Kafka")
Rel(indexer, os, "Upsert documents", "HTTP/Bulk")

Rel(metadata, config, "Get retention policy", "HTTP")
Rel(metadata, retention, "Trigger evaluation", "HTTP")
Rel(retention, pg, "Find versions > N", "SQL")
Rel(retention, archive, "Archive requests", "HTTP")
Rel(archive, nfs, "Move binaries to archive namespace", "NFS")
Rel(archive, pg, "Mark versions archived; recompute latest", "SQL")
Rel(archive, kafka, "Publish Archived", "Kafka")

Rel(metadata, oidc, "Validate tokens for Search API", "OIDC/JWT")

@enduml
```

Sources: `PartB/Diagrams/C4_SystemContext.puml`, `PartB/Diagrams/C4_Container.puml`.
