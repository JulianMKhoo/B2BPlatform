"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/lib/auth-context";
import { chat, chatAccess, businessUnits } from "@/lib/api";
import type { BusinessUnitItem } from "@/lib/types";

interface Message {
  message_id: number;
  sender_id: number;
  message_text: string;
  sent_at: string;
}

export default function ChatPage() {
  const { ownerId, employeeId, companyId, unitId: ctxUnitId, isEmployee, canManage } = useAuth();
  const senderId = employeeId || ownerId;

  const [unitId, setUnitId] = useState("");
  const [workspace, setWorkspace] = useState<{ workspace_id: number; workspace_name: string } | null>(null);
  const [messages, setMessages] = useState<Message[]>([]);
  const [newMsg, setNewMsg] = useState("");
  const [error, setError] = useState("");
  const [autoLoaded, setAutoLoaded] = useState(false);
  const [buList, setBuList] = useState<BusinessUnitItem[]>([]);

  // Access control (owner only)
  const [grantEmpId, setGrantEmpId] = useState("");
  const [accessMsg, setAccessMsg] = useState("");

  // Load BU list for owner dropdown
  useEffect(() => {
    if (!canManage || !companyId) return;
    businessUnits.list(companyId).then((res) => setBuList(res.businessUnits || []));
  }, [canManage, companyId]);

  // Auto-load workspace for employees (fixed unitId)
  useEffect(() => {
    if (autoLoaded || !isEmployee || !ctxUnitId) return;
    setAutoLoaded(true);
    (async () => {
      const res = await chat.getWorkspace(ctxUnitId);
      if (!("error" in res)) {
        setUnitId(String(ctxUnitId));
        setWorkspace(res);
        const msgRes = await chat.getMessages(res.workspace_id);
        setMessages(msgRes.messages || []);
      }
    })();
  }, [autoLoaded, isEmployee, ctxUnitId]);

  // Auto-load first workspace for owners/admins once buList is ready
  useEffect(() => {
    if (autoLoaded || !canManage || buList.length === 0) return;
    setAutoLoaded(true);
    const firstBuId = buList[0].id;
    (async () => {
      const res = await chat.getWorkspace(firstBuId);
      if (!("error" in res)) {
        setUnitId(String(firstBuId));
        setWorkspace(res);
        const msgRes = await chat.getMessages(res.workspace_id);
        setMessages(msgRes.messages || []);
      }
    })();
  }, [autoLoaded, canManage, buList]);

  const loadWorkspace = async (selectedUnitId: number) => {
    setError("");
    setWorkspace(null);
    setMessages([]);
    const res = await chat.getWorkspace(selectedUnitId);
    if ("error" in res) {
      setError(typeof res.error === "string" ? res.error : "Workspace not found");
      return;
    }
    setWorkspace(res);
    const msgRes = await chat.getMessages(res.workspace_id);
    setMessages(msgRes.messages || []);
  };

  const handleBuChange = (val: string) => {
    setUnitId(val);
    if (val) loadWorkspace(+val);
  };

  const sendMessage = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!workspace || !newMsg.trim() || !senderId) return;
    await chat.sendMessage({
      workspace_id: workspace.workspace_id,
      sender_id: senderId,
      message_text: newMsg,
    });
    setNewMsg("");
    const msgRes = await chat.getMessages(workspace.workspace_id);
    setMessages(msgRes.messages || []);
  };

  const handleGrant = async () => {
    if (!grantEmpId || !unitId) return;
    setAccessMsg("");
    await chatAccess.grant({ unitId: +unitId, employeeId: +grantEmpId });
    setAccessMsg(`Access granted for employee #${grantEmpId}`);
    setGrantEmpId("");
  };

  const handleRevoke = async () => {
    if (!grantEmpId || !unitId) return;
    setAccessMsg("");
    await chatAccess.revoke({ unitId: +unitId, employeeId: +grantEmpId });
    setAccessMsg(`Access revoked for employee #${grantEmpId}`);
    setGrantEmpId("");
  };

  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">Chat Workspaces</h1>

      {canManage && (
        <div className="mb-6">
          <label className="block text-sm font-medium mb-1">Business Unit</label>
          <select
            value={unitId}
            onChange={(e) => handleBuChange(e.target.value)}
            className="px-3 py-2 border border-border rounded-lg text-sm w-64 bg-white"
          >
            <option value="">Select a Business Unit...</option>
            {buList.map((bu) => (
              <option key={bu.id} value={bu.id}>{bu.businessUnitName}</option>
            ))}
          </select>
        </div>
      )}

      {error && <p className="text-danger text-sm mb-4">{error}</p>}

      {workspace && (
        <div className={`grid grid-cols-1 ${canManage ? "lg:grid-cols-3" : ""} gap-6`}>
          {/* Chat area */}
          <div className={`${canManage ? "lg:col-span-2" : ""} bg-card border border-border rounded-xl flex flex-col h-[500px]`}>
            <div className="px-4 py-3 border-b border-border">
              <h2 className="font-semibold">{workspace.workspace_name}</h2>
              <p className="text-xs text-muted">Workspace #{workspace.workspace_id}</p>
            </div>

            <div className="flex-1 overflow-y-auto p-4 space-y-3">
              {messages.length === 0 ? (
                <p className="text-muted text-sm text-center mt-8">No messages yet</p>
              ) : (
                [...messages].reverse().map((m) => (
                  <div
                    key={m.message_id}
                    className={`max-w-[70%] px-3 py-2 rounded-lg text-sm ${
                      m.sender_id === senderId
                        ? "bg-primary text-white ml-auto"
                        : "bg-gray-100 text-foreground"
                    }`}
                  >
                    <p className="text-xs opacity-70 mb-0.5">
                      {m.sender_id === senderId ? "You" : `Employee #${m.sender_id}`}
                    </p>
                    <p>{m.message_text}</p>
                  </div>
                ))
              )}
            </div>

            <form onSubmit={sendMessage} className="p-3 border-t border-border flex gap-2">
              <input
                value={newMsg}
                onChange={(e) => setNewMsg(e.target.value)}
                placeholder="Type a message..."
                className="flex-1 px-3 py-2 border border-border rounded-lg text-sm"
              />
              <button
                type="submit"
                className="bg-primary text-white px-4 py-2 rounded-lg text-sm hover:bg-primary-hover"
              >
                Send
              </button>
            </form>
          </div>

          {/* Access control panel - owner & admin */}
          {canManage && (
            <div className="bg-card border border-border rounded-xl p-4">
              <h3 className="font-semibold mb-4">Access Control</h3>
              <p className="text-xs text-muted mb-3">Grant or revoke chat access for employees in this BU</p>

              <div className="space-y-3">
                <div>
                  <label className="block text-xs font-medium mb-1">Employee ID</label>
                  <input
                    type="number"
                    value={grantEmpId}
                    onChange={(e) => setGrantEmpId(e.target.value)}
                    className="w-full px-3 py-2 border border-border rounded-lg text-sm"
                    placeholder="Employee ID"
                  />
                </div>
                <div className="flex gap-2">
                  <button
                    onClick={handleGrant}
                    className="flex-1 bg-success text-white py-2 rounded-lg text-sm hover:opacity-90"
                  >
                    Grant
                  </button>
                  <button
                    onClick={handleRevoke}
                    className="flex-1 bg-danger text-white py-2 rounded-lg text-sm hover:opacity-90"
                  >
                    Revoke
                  </button>
                </div>
                {accessMsg && <p className="text-xs text-success">{accessMsg}</p>}
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
