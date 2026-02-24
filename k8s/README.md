# Kubernetes Deployment

## Prerequisites

- Kubernetes cluster (EKS, GKE, AKS, or on-prem)
- `kubectl` configured
- Docker image pushed to registry

## Setup

1. **Update deployment image**: Replace `DOCKERHUB_USER` in `deployment.yaml` with your Docker Hub username, or let CD workflow do it via sed.

2. **Create Secret** with real values (do not commit):
   ```bash
   kubectl create secret generic monitoring-secret \
     --from-literal=ConnectionStrings__DefaultConnection='Data Source=/data/app.db' \
     --from-literal=Jwt__Key='your-32-byte-jwt-secret' \
     --dry-run=client -o yaml | kubectl apply -f -
   ```

3. **Apply manifests** (order matters for dependencies):
   ```bash
   kubectl apply -f configmap.yaml
   kubectl apply -f secret.yaml   # or use above
   kubectl apply -f service.yaml
   kubectl apply -f deployment.yaml
   ```

## Redis

This app expects Redis at `redis:6379`. Add a Redis Deployment + Service in the same namespace, or use an external Redis and update `ConnectionStrings__Redis` in ConfigMap.

## SQLite & Replicas

- **SQLite + ReadWriteOnce PVC**: Use `replicas: 1` (current default).
- For 3+ replicas with zero downtime: migrate to PostgreSQL and use external DB; then increase replicas and remove the PVC from deployment.

## GitHub Secrets (CD)

- `KUBECONFIG`: Base64-encoded kubeconfig. `cat ~/.kube/config | base64 -w0`
- `DOCKER_USERNAME`: For image reference in deployment
