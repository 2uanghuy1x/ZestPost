import React, { useState, useEffect } from 'react';
import { csharpApi } from '../../api';
import './GroupAccountCrud.css';

function GroupAccountCrud() {
    const [groupAccounts, setGroupAccounts] = useState([]);

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'groupAccountsData') {
                setGroupAccounts(message.payload);
            }
            if (message.action === 'actionSuccess') {
                // Re-fetch group accounts after a successful action (e.g., deletion)
                csharpApi.getGroupAccounts();
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getGroupAccounts();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    const handleDelete = (groupAccount) => {
        if (window.confirm(`Bạn có chắc chắn muốn xóa tài khoản nhóm ${groupAccount.name} này không?`)) {
            csharpApi.deleteGroupAccount(groupAccount);
        }
    };

    return (
        <div className="crud-container">
            <div className="card">
                <div className="card-header">
                    <h3>Quản lý Tài khoản Nhóm</h3>
                </div>
                <div className="table-container">
                    <table className="crud-table">
                        <thead>
                            <tr>
                                <th>Tên Nhóm</th>
                                <th>Group ID</th>
                                <th className="actions-column">Hành động</th>
                            </tr>
                        </thead>
                        <tbody>
                            {groupAccounts.map((groupAccount) => (
                                <tr key={groupAccount.id}>
                                    <td>{groupAccount.name}</td>
                                    <td>{groupAccount.groupId}</td>
                                    <td className="actions-column">
                                        <button className="action-btn delete" onClick={() => handleDelete(groupAccount)}>Xóa</button>
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

export default GroupAccountCrud;
