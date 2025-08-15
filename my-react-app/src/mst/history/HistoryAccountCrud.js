
import React, { useState, useEffect } from 'react';
import { csharpApi } from '../../api';
import './HistoryAccountCrud.css';

function HistoryAccountCrud() {
    const [historyAccounts, setHistoryAccounts] = useState([]);

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'historyAccountsData') {
                setHistoryAccounts(message.payload);
            }
            if (message.action === 'actionSuccess') {
                // Re-fetch history accounts after a successful action (e.g., deletion)
                csharpApi.getHistoryAccounts();
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getHistoryAccounts();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    const handleDelete = (historyAccount) => {
        if (window.confirm(`Bạn có chắc chắn muốn xóa mục lịch sử ${historyAccount.id} này không?`)) {
            csharpApi.deleteHistoryAccount(historyAccount);
        }
    };

    return (
        <div className="crud-container">
            <div className="card">
                <div className="card-header">
                    <h3>Quản lý Lịch sử Tài khoản</h3>
                </div>
                <div className="table-container">
                    <table className="crud-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Account ID</th>
                                <th>Hành động</th>
                                <th>Thời gian</th>
                                <th className="actions-column">Hành động</th>
                            </tr>
                        </thead>
                        <tbody>
                            {historyAccounts.map((historyAccount) => (
                                <tr key={historyAccount.id}>
                                    <td>{historyAccount.id}</td>
                                    <td>{historyAccount.accountId}</td>
                                    <td>{historyAccount.action}</td>
                                    <td>{new Date(historyAccount.timestamp).toLocaleString()}</td>
                                    <td className="actions-column">
                                        <button className="action-btn delete" onClick={() => handleDelete(historyAccount)}>Xóa</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}

export default HistoryAccountCrud;
