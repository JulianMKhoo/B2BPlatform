"use client";

import { useState, useEffect } from "react";
import { useAuth } from "@/lib/auth-context";
import { auth, employees } from "@/lib/api";
import { ROLE_LABELS } from "@/lib/types";
import type { Employee } from "@/lib/types";

export default function ProfilePage() {
  const { isOwner, ownerId, employeeId, companyId, unitId, employeeRole, email, firstName, lastName } = useAuth();

  const [employee, setEmployee] = useState<Employee | null>(null);
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!isOwner && employeeId) {
      employees.get(employeeId).then((res: unknown) => {
        const data = res as { employee?: Employee; status?: { code: string } };
        if (data.employee) setEmployee(data.employee);
      });
    }
  }, [isOwner, employeeId]);

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setSuccess("");

    if (newPassword !== confirmPassword) {
      setError("New passwords do not match");
      return;
    }

    if (newPassword.length < 4) {
      setError("New password must be at least 4 characters");
      return;
    }

    setLoading(true);
    try {
      const res = isOwner
        ? await auth.changeOwnerPassword({ id: ownerId!, currentPassword, newPassword })
        : await auth.changeEmployeePassword({ employeeId: employeeId!, currentPassword, newPassword });

      if (res.status?.code !== "200") {
        setError(res.status?.message || "Failed to change password");
      } else {
        setSuccess("Password changed successfully");
        setCurrentPassword("");
        setNewPassword("");
        setConfirmPassword("");
      }
    } catch {
      setError("Connection failed");
    } finally {
      setLoading(false);
    }
  };

  const roleName = employeeRole != null ? (ROLE_LABELS[employeeRole] || "Employee") : null;

  return (
    <div className="max-w-lg space-y-6">
      <h1 className="text-2xl font-bold">Profile</h1>

      <div className="bg-card border border-border rounded-xl p-6 space-y-3">
        <h2 className="font-semibold text-lg">Account Info</h2>
        {isOwner ? (
          <div className="space-y-2 text-sm">
            <div className="flex justify-between">
              <span className="text-muted">Owner ID</span>
              <span className="font-medium">{ownerId}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-muted">Company ID</span>
              <span className="font-medium">{companyId}</span>
            </div>
          </div>
        ) : (
          <div className="space-y-2 text-sm">
            <div className="flex justify-between">
              <span className="text-muted">Employee ID</span>
              <span className="font-medium">{employeeId}</span>
            </div>
            {(firstName || employee) && (
              <div className="flex justify-between">
                <span className="text-muted">Name</span>
                <span className="font-medium">
                  {firstName && lastName ? `${firstName} ${lastName}` : employee ? `${employee.firstName} ${employee.lastName}` : ""}
                </span>
              </div>
            )}
            {email && (
              <div className="flex justify-between">
                <span className="text-muted">Email</span>
                <span className="font-medium">{email}</span>
              </div>
            )}
            {roleName && (
              <div className="flex justify-between">
                <span className="text-muted">Role</span>
                <span className={`px-2 py-0.5 rounded text-xs font-medium ${
                  employeeRole === 1 ? "bg-blue-100 text-blue-700" : "bg-gray-100 text-gray-700"
                }`}>
                  {roleName}
                </span>
              </div>
            )}
            <div className="flex justify-between">
              <span className="text-muted">Business Unit</span>
              <span className="font-medium">{unitId}</span>
            </div>
          </div>
        )}
      </div>

      <div className="bg-card border border-border rounded-xl p-6">
        <h2 className="font-semibold text-lg mb-4">Change Password</h2>

        {error && (
          <div className="bg-red-50 text-danger text-sm px-4 py-2 rounded-lg mb-4">{error}</div>
        )}
        {success && (
          <div className="bg-green-50 text-green-700 text-sm px-4 py-2 rounded-lg mb-4">{success}</div>
        )}

        <form onSubmit={handleChangePassword} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">Current Password</label>
            <input
              type="password"
              value={currentPassword}
              onChange={(e) => setCurrentPassword(e.target.value)}
              required
              className="w-full px-3 py-2 border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">New Password</label>
            <input
              type="password"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              required
              className="w-full px-3 py-2 border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Confirm New Password</label>
            <input
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
              className="w-full px-3 py-2 border border-border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <button
            type="submit"
            disabled={loading}
            className="w-full bg-primary text-white py-2 rounded-lg hover:bg-primary-hover disabled:opacity-50 transition-colors font-medium"
          >
            {loading ? "Changing..." : "Change Password"}
          </button>
        </form>
      </div>
    </div>
  );
}
