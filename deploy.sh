#!/usr/bin/env bash

# Xóa cache build Docker
docker builder prune -f

# Xóa các image không cần thiết
docker image prune -a -f

# Xóa các container đã dừng
docker container prune -f

# Xóa các volume không cần thiết
docker volume prune -f

# Dừng các container đang chạy mà không xóa network
docker-compose -f WebApp/docker-compose.yaml stop

# Xóa các container cũ
docker-compose -f WebApp/docker-compose.yaml rm -f

# Pull image mới từ Docker Hub
docker-compose -f WebApp/docker-compose.yaml pull

# Chạy container mới
docker-compose -f WebApp/docker-compose.yaml up -d