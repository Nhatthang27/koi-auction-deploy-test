const express = require("express");
const { createProxyMiddleware } = require("http-proxy-middleware");
const cors = require("cors");
const helmet = require("helmet");
const morgan = require("morgan");
const rateLimit = require("express-rate-limit");
const cookieParser = require("cookie-parser");
const { verify } = require("jsonwebtoken");

require("dotenv").config();

const app = express();

// Middleware setup
const corsOptions = {
  origin: [
    "http://localhost:5173",
    "http://localhost:3001",
    "http://localhost:3002",
    "http://localhost:3003",
  ],
  credentials: true,
};
app.use(cors(corsOptions));
app.use(helmet()); // Add security headers
app.use(morgan("combined")); // Log HTTP requests
app.use(cookieParser()); // Parse cookies
app.disable("x-powered-by"); // Hide Express server information

// Define routes and corresponding microservices
const services = [
  {
    route: "/user-service",
    target: "localhost:3001/api",
  },
  {
    route: "/auction-service",
    target: "localhost:3002/api",
  },
  {
    route: "/bidding-service",
    target: "localhost:3003/api",
  },
  {
    route: "/payment-service",
    target: "localhost:3004/api",
  },
];

const limiter = rateLimit({
  windowMs: 1 * 60 * 1000, // 1 minute
  max: 100, // 100 requests per minute
  message: "Too many requests from this IP, please try again after a minute",
});

const authentification = (req, res, next) => {
  if (!req.cookies) {
    console.log("No cookies");
    return next();
  }

  const accessToken = req.cookies["access-token"];
  if (!accessToken) {
    return next();
  }

  try {
    verify(accessToken, process.env.JWT_SECRET, (err, decoded) => {
      if (err) {
        return next();
      }
      console.log("Decoded", decoded);
      req.headers.uid = decoded.UserId;
      req.headers.uri = decoded.UserRoleId;
      next();
    });
  } catch (error) {
    console.log("Error");
    next();
  }
};

services.forEach(({ route, target }) => {
  console.log(`Proxying ${route} to ${target}`);
  const proxyOptions = {
    target: `http://${target}`,
    pathRewrite: {
      [`^/${route}`]: "",
    },
  };
  app.use(
    route,
    limiter,
    authentification,
    createProxyMiddleware(proxyOptions)
  );
});

const port = process.env.PORT || 3000;

// Start the API Gateway server
app.listen(port, () => {
  console.log(`API Gateway is running on 127.0.0.1:${port}`);
});
