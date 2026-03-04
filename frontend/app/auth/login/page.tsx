"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { auth } from "@/lib/api";
import { useAuth } from "@/lib/auth-context";

type Tab = "owner" | "employee";

export default function LoginPage() {
  const [tab, setTab] = useState<Tab>("owner");
  const [ownerId, setOwnerId] = useState("");
  const [password, setPassword] = useState("");
  const [email, setEmail] = useState("");
  const [empPassword, setEmpPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const router = useRouter();
  const { loginAsOwner, loginAsEmployee } = useAuth();

  const handleOwnerLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      const res = await auth.ownerLogin({ id: +ownerId, password });
      if (res.status?.code !== "200") {
        setError(res.status?.message || "Login failed");
        return;
      }
      loginAsOwner(res.id, res.companyId);
      router.push("/dashboard");
    } catch {
      setError("Connection failed");
    } finally {
      setLoading(false);
    }
  };

  const handleEmployeeLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      const res = await auth.employeeLogin({ email, password: empPassword });
      if (res.status?.code !== "200") {
        setError(res.status?.message || "Login failed");
        return;
      }
      loginAsEmployee(res.id, res.companyId, res.unitId, res.role, email, res.firstName, res.lastName);
      router.push(res.role === 1 ? "/dashboard" : "/dashboard/chat");
    } catch {
      setError("Connection failed");
    } finally {
      setLoading(false);
    }
  };

  const tabClass = (t: Tab) =>
    `flex-1 py-2 text-sm font-medium text-center rounded-lg transition-colors ${
      tab === t ? "bg-primary text-white" : "text-muted hover:text-foreground"
    }`;

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-full max-w-md bg-card border border-border rounded-xl p-8 shadow-sm">
        <h1 className="text-2xl font-bold mb-1">Sign In</h1>
        <p className="text-muted mb-6">Choose your account type</p>

        <div className="flex gap-2 mb-6 bg-gray-100 p-1 rounded-lg">
          <button className={tabClass("owner")} onClick={() => { setTab("owner"); setError(""); }}>
            Owner Login
          </button>
          <button className={tabClass("employee")} onClick={() => { setTab("employee"); setError(""); }}>
            Employee Login
          </button>
        </div>

        {error && (
          <div className="bg-red-50 text-danger text-sm px-4 py-2 rounded-lg mb-4">{error}</div>
        )}

        {tab === "owner" ? (
          <form onSubmit={handleOwnerLogin} className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1">Owner ID</label>
              <input
                type="number"
                value={ownerId}
                onChange={(e) => setOwnerId(e.target.value)}
                required
                className="w-full px-3 py-2 border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Enter your Owner ID"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Password</label>
              <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                className="w-full px-3 py-2 border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-primary text-white py-2 rounded-lg hover:bg-primary-hover disabled:opacity-50 transition-colors font-medium"
            >
              {loading ? "Signing in..." : "Sign In as Owner"}
            </button>
          </form>
        ) : (
          <form onSubmit={handleEmployeeLogin} className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1">Email</label>
              <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                className="w-full px-3 py-2 border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Enter your email"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Password</label>
              <input
                type="password"
                value={empPassword}
                onChange={(e) => setEmpPassword(e.target.value)}
                required
                className="w-full px-3 py-2 border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-primary text-white py-2 rounded-lg hover:bg-primary-hover disabled:opacity-50 transition-colors font-medium"
            >
              {loading ? "Signing in..." : "Sign In as Employee"}
            </button>
          </form>
        )}

        <p className="text-sm text-muted text-center mt-6">
          No account?{" "}
          <Link href="/auth/register" className="text-primary hover:underline">
            Register your company
          </Link>
        </p>
      </div>
    </div>
  );
}
