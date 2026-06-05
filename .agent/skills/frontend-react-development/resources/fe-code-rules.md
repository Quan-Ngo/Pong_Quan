# Frontend Code Rules

> Coding conventions for the **cnma-boilerplate** design system and all downstream apps.

---

## 1. Component Styling — CVA First

### ✅ Rule: Use CVA variants, never duplicate color maps

```tsx
// ✅ GOOD — use the shared CVA component
import { StatusBadge } from "@/components/common/StatusBadge";
<StatusBadge status="Approved" />

// ❌ BAD — defining your own color map per page
const STATUS_COLORS: Record<string, string> = {
  "Approved": "bg-status-released text-status-released-text ...",
};
<Badge className={STATUS_COLORS[status]} variant="outline">{status}</Badge>
```

### ✅ Rule: Use Button variants, never inline brand colors

```tsx
// ✅ GOOD — use existing variant
<Button variant="create">Save</Button>
<Button variant="success">Saved</Button>
<Button variant="action">Submit</Button>

// ❌ BAD — hardcoding CSS variables
<Button className="bg-[var(--ds-brand-red-6)] text-white">Save</Button>
```

### ✅ Rule: Use design system components, never raw HTML equivalents

| Instead of | Use |
|:---|:---|
| `<textarea>` | `<Textarea>` from `@/components/ui/textarea` |
| `<input>` | `<Input>` from `@/components/ui/input` |
| `<button>` | `<Button>` from `@/components/ui/button` |
| `<select>` | `<Select>` from `@/components/ui/select` |
| Manual avatar `<div>` | `<Avatar>` from `@/components/ui/avatar` |
| Manual progress `<div>` | `<Progress>` from `@/components/ui/progress` |

---

## 2. Status & Priority Badges

### ✅ Rule: Always use shared badge components

```tsx
import { StatusBadge } from "@/components/common/StatusBadge";
import { PriorityBadge } from "@/components/common/PriorityBadge";

// Status — auto-maps to theme colors
<StatusBadge status="Approved" />    // → green (released)
<StatusBadge status="In Progress" /> // → blue (progress)
<StatusBadge status="Rejected" />    // → red (obsoleted)

// Priority — auto-maps to severity colors
<PriorityBadge priority="Critical" />  // → red
<PriorityBadge priority="High" />      // → orange
<PriorityBadge priority="Medium" />    // → blue
<PriorityBadge priority="Low" />       // → gray
```

### ✅ Rule: Add new status mappings to the shared component, not the page

If a new status string appears (e.g. `"On Hold"`), add it to `STATUS_VARIANT_MAP` in `StatusBadge.tsx`, **not** to a local map in the page.

### ✅ Rule: Creating Domain-Specific Status Badges

When creating a new badge component for the statuses of a specific business object (e.g. `PurchaseOrderStatusBadge`, `InvoiceStatusBadge`), you must create it centrally inside `@/components/common/` (or equivalent shared folder). 

**Do not** invent a local badge inside the page's directory. 
Your new domain-specific badge must consume the `statusBadgeVariants` exported by `@cnma/react-ui` and map your domain terminology to the library's semantic colors (`new`, `progress`, `released`, `approved`, `sent`, `completed`, `obsoleted`), exactly like the sample `StatusBadge` implemented in the library.

---

## 3. Theme Colors & Shared Design System

### ✅ Rule: Use semantic CSS variables from the library's `theme.css`

> **Note for AI and Developers**: The core CSS design tokens are now bundled in the `@cnma/react-ui` library. To discover the available class names, CSS variables, and tailwind utilities, you should inspect `node_modules/@cnma/react-ui/dist/theme.css` or `node_modules/@cnma/react-ui/dist/styles.css`.

```tsx
// ✅ GOOD — semantic variables
className="bg-status-released text-status-released-text"
className="text-primary"
className="bg-muted text-muted-foreground"

// ❌ BAD — raw CSS variable references
className="bg-[var(--ds-brand-red-6)]"
className="text-[var(--ds-brand-red-7)]"

// ❌ BAD — hardcoded Tailwind colors
className="bg-emerald-600 text-white"
className="bg-red-500"
```

**Exception**: Raw CSS variable references are acceptable in design system primitives (e.g., `button.tsx`, `input.tsx`), but **never** in page-level code.

---

## 4. DataTable Columns

### ✅ Rule: Use `renderType: "custom"` with shared components

```tsx
// ✅ GOOD — delegate to StatusBadge
{
  key: "status",
  labelKey: "Status",
  renderType: "custom",
  render: (v: string) => <StatusBadge status={v} />
}

// ❌ BAD — inline Badge with color map lookup
{
  key: "status",
  render: (v: string) => (
    <Badge className={`${STATUS_COLORS[v]} border`} variant="outline">{v}</Badge>
  )
}
```

---

## 5. Adding New CVA Components

When creating a new component that needs variants:

```tsx
import { cva, type VariantProps } from "class-variance-authority";
import { cn } from "@/lib/utils";

const myComponentVariants = cva(
  "base-classes-here",
  {
    variants: {
      variant: {
        default: "...",
        primary: "...",
      },
      size: {
        sm: "...",
        md: "...",
      },
    },
    defaultVariants: {
      variant: "default",
      size: "md",
    },
  }
);

interface MyComponentProps extends VariantProps<typeof myComponentVariants> {
  className?: string;
  children: React.ReactNode;
}

export function MyComponent({ variant, size, className, children }: MyComponentProps) {
  return (
    <div className={cn(myComponentVariants({ variant, size }), className)}>
      {children}
    </div>
  );
}
```

---

## 6. Imports & Exports

### ✅ Rule: Import from barrel files when available

```tsx
// ✅ GOOD — import from common barrel
import { StatusBadge, PriorityBadge } from "@/components/common";

// ✅ ALSO GOOD — direct import
import { StatusBadge } from "@/components/common/StatusBadge";

// ❌ BAD — importing from internal paths
import { statusBadgeVariants } from "@/components/common/StatusBadge";
// (only if you truly need the raw CVA function)
```

### ✅ Rule: Clean up unused imports

Remove unused imports (components, hooks, types) when refactoring. TypeScript will warn about these with `TS6133`.

---

## 7. Reusable Patterns Checklist

Before writing page-specific styling, check if a shared component already exists:

| Pattern | Component |
|:---|:---|
| Status indicator | `StatusBadge` |
| Priority/risk indicator | `PriorityBadge` |
| Data table | `DataTable` |
| Filters | `FilterBar` |
| Table actions toolbar | `TableToolbar` |
| Form field with label | `Label` + `Input`/`Select`/`Textarea` |
| Confirmation dialog | `AlertDialog` |
| Side panel | `Sheet` |
| Loading skeleton | `Skeleton` |
| Toast notification | `toast()` from sonner |

If you need a pattern that doesn't exist, **create a reusable CVA component** in `components/common/` or `components/ui/` rather than building it inline in the page.

---

## 8. Internationalization (i18n)

### ✅ Rule: Always use i18n for text elements in the UI

Hardcoding strings in the UI makes the application untranslatable and difficult to maintain. Always use the `useTranslation` hook for text inside pages and components.

```tsx
// ✅ GOOD — using i18n hook
import { useTranslation } from 'react-i18next';

export function AnalyticsPage() {
  const { t } = useTranslation();
  
  return (
    <h1>{t('pages.BusinessAnalytics.ASNAnalytics.asnBusinessObject')}</h1>
  );
}

// ❌ BAD — hardcoding text
export function AnalyticsPage() {
  return (
    <h1>ASN Business Object Data</h1>
  );
}
```
