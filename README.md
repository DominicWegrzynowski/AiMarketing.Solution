# Prompt-n-Post

**AI-powered social media marketing SaaS built with Blazor and ASP.NET Core Web API.**

[Live Application](https://prompt-n-post.com/) | [Developer Portfolio](https://dominicwegrzynowski.dev)

## Overview

Prompt-n-Post is a full-stack SaaS application designed to help small businesses generate personalized marketing content using artificial intelligence.

The application uses **Blazor** for its user interface and **ASP.NET Core Web API** for backend functionality, integrating the **OpenAI API** for content generation and **Stripe** for subscription billing.

Users enter information about their business through a form, which is used to construct requests to OpenAI's API and generate marketing content tailored to their business.

I developed Prompt-n-Post independently to explore full-stack .NET development, generative AI integration, and subscription-based SaaS architecture.

## Tech Stack

| Component | Technology |
|---|---|
| Frontend | Blazor |
| Backend | C#, ASP.NET Core Web API |
| AI Integration | OpenAI API |
| Subscription Billing | Stripe |
| Architecture | Full-stack .NET, API-based integration |

## Key Features

### AI-Powered Marketing Content
- Generates personalized marketing content using the OpenAI API.
- Accepts business-specific information through a structured input form.
- Uses the submitted information to provide context for AI-generated content.
- Presents generated content for user review.

### Subscription Billing
- Integrates Stripe for paid subscriptions.
- Supports a subscription-based SaaS business model.
- Incorporates payment processing into the application experience.

### Business-Focused Interface
- Blazor-based frontend for collecting business information.
- Straightforward content generation workflow.
- Designed for small-business owners seeking assistance with social media marketing.

## Application Architecture

The application separates its user interface, backend services, and external integrations.

```text
┌────────────────────────────┐
│       Blazor Frontend      │
│ Business Information Form  │
└─────────────┬──────────────┘
              │
              ▼
┌────────────────────────────┐
│    ASP.NET Core Web API    │
│   Application Logic        │
└─────────────┬──────────────┘
              │
       ┌──────┴───────┐
       ▼              ▼
┌──────────────┐ ┌──────────────┐
│  OpenAI API  │ │  Stripe API  │
│  Marketing   │ │ Subscription │
│  Generation  │ │   Billing    │
└──────────────┘ └──────────────┘
```

## How It Works

1. **Enter Business Information** — Users provide details about their business through a Blazor form.
2. **Submit Generation Request** — The application sends the relevant input through its backend API.
3. **Generate Content** — The backend integrates with the OpenAI API to request personalized marketing content.
4. **Review Results** — Generated content is returned to the application for the user to review.
5. **Manage Subscriptions** — Stripe provides subscription billing functionality for paid access.

## Engineering Highlights

Prompt-n-Post demonstrates practical experience with:

- Developing a full-stack application using C# and .NET.
- Building interactive interfaces with Blazor.
- Implementing backend functionality with ASP.NET Core Web API.
- Integrating external REST APIs into application workflows.
- Using generative AI to solve a business-oriented problem.
- Integrating Stripe for subscription billing.
- Developing and deploying a publicly accessible SaaS product.

## Live Application

**https://prompt-n-post.com/**

## Project Background

I built Prompt-n-Post as an independent project to explore how generative AI could be applied to a practical small-business problem.

The goal was to create more than a proof of concept: a usable application combining a modern .NET technology stack, AI-powered functionality, and subscription billing.

## Author

**Dominic Wegrzynowski**  
Full-Stack Software Developer

- [Portfolio](https://dominicwegrzynowski.dev)
- [LinkedIn](https://www.linkedin.com/in/dominic-wegrzynowski/)
- [Email](mailto:dominicwegrzynowski@gmail.com)
