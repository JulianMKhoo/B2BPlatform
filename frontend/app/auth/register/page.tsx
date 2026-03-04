"use client";

import { useState } from "react";
import Link from "next/link";
import { auth } from "@/lib/api";

export default function RegisterPage() {
  const [form, setForm] = useState({ companyName: "", companyAddress: "", contactNumber: "" });
  const [result, setResult] = useState<{ ownerId: number; defaultPassword: string } | null>(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const update = (key: string, value: string) => setForm((f) => ({ ...f, [key]: value }));

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setResult(null);
    setLoading(true);
    try {
      const res = await auth.register(form);
      if (res.status?.code !== "200") {
        setError(res.status?.message || "Registration failed");
        return;
      }
      setResult({ ownerId: res.ownerId, defaultPassword: res.defaultPassword });
    } catch {
      setError("Connection failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background">
      <div className="w-full max-w-md bg-card border border-border rounded-xl p-8 shadow-sm">
        <h1 className="text-2xl font-bold mb-1">Company Onboarding</h1>
        <p className="text-muted mb-6">Register your company to get started</p>

        {error && (
          <div className="bg-red-50 text-danger text-sm px-4 py-2 rounded-lg mb-4">{error}</div>
        )}

        {result ? (
          <div className="space-y-4">
            <div className="bg-green-50 border border-green-200 rounded-lg p-4">
              <p className="text-sm font-medium text-green-800 mb-2">Company registered successfully!</p>
              <p className="text-sm text-green-700">Use these credentials to sign in:</p>
              <div className="mt-3 space-y-2 bg-white rounded-lg p-3 border border-green-100">
                <div className="flex justify-between text-sm">
                  <span className="text-muted">Owner ID:</span>
                  <span className="font-mono font-semibold">{result.ownerId}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted">Password:</span>
                  <span className="font-mono font-semibold">{result.defaultPassword}</span>
                </div>
              </div>
            </div>
            <Link
              href="/auth/login"
              className="block w-full text-center bg-primary text-white py-2 rounded-lg hover:bg-primary-hover transition-colors font-medium"
            >
              Go to Sign In
            </Link>
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1">Company Name</label>
              <input
                value={form.companyName}
                onChange={(e) => update("companyName", e.target.value)}
                required
                className="w-full px-3 py-2 border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="Acme Corp"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Company Address</label>
              <input
                value={form.companyAddress}
                onChange={(e) => update("companyAddress", e.target.value)}
                className="w-full px-3 py-2 border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="123 Main St, Bangkok"
              />
            </div>
            <div>
              <label className="block text-sm font-medium mb-1">Contact Number</label>
              <input
                value={form.contactNumber}
                onChange={(e) => update("contactNumber", e.target.value)}
                className="w-full px-3 py-2 border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                placeholder="02-123-4567"
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-primary text-white py-2 rounded-lg hover:bg-primary-hover disabled:opacity-50 transition-colors font-medium"
            >
              {loading ? "Registering..." : "Register Company"}
            </button>
          </form>
        )}

        <p className="text-sm text-muted text-center mt-6">
          Already registered?{" "}
          <Link href="/auth/login" className="text-primary hover:underline">
            Sign in
          </Link>
        </p>
      </div>
    </div>
  );
}
