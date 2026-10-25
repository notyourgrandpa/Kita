# =========================
# Stage 1: Build Angular
# =========================

FROM node:20-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm ci

COPY . .

RUN npm run build


# =========================
# Stage 2: Serve with Nginx
# =========================

FROM nginx:alpine AS final

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist/kita.client/browser /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
