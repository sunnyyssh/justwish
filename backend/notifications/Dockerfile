# Use the official Golang image as a builder
FROM golang:1.23 AS builder

# Set the working directory inside the container
WORKDIR /app

# Copy the Go modules manifests and download dependencies
COPY go.mod go.sum ./
RUN go mod download

# Copy the application source code
COPY . .

# Build the Go application
RUN go build -o main .

# Use a minimal base image for the final stage
FROM ubuntu

# Set the working directory
WORKDIR /app

# Copy the built binary from the builder
COPY --from=builder /app/main .

# Expose application port (if applicable, change it to your app's port)
EXPOSE 5002

# Command to run the executable
CMD ["./main"]
