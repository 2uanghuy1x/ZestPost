import React, { useState, useEffect } from 'react';
import { csharpApi } from '../../api';
import './GroupAccountCrud.css';

function GroupAccountCrud() {
    const [groupAccounts, setGroupAccounts] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [currentGroupAccount, setCurrentGroupAccount] = useState(null);

    useEffect(() => {
        const handleMessage = (event) => {
            const message = event.data;
            if (message.action === 'groupAccountsData') {
                setGroupAccounts(message.payload);
            }
            if (message.action === 'actionSuccess') {
                setIsModalOpen(false);
                setCurrentGroupAccount(null);
            }
        };

        csharpApi.addEventListener('message', handleMessage);
        csharpApi.getGroupAccounts();

        return () => {
            csharpApi.removeEventListener('message', handleMessage);
        };
    }, []);

    const handleAddNew = () => {
        setCurrentGroupAccount({
            name: '',
            groupId: '',
            categoryId: null
        });
        setIsModalOpen(true);
    };

    const handleEdit = (groupAccount) => {
        setCurrentGroupAccount(groupAccount);
        setIsModalOpen(true);
    };

    const handleDelete = (groupAccount) => {
        if (window.confirm(`Are you sure you want to delete group account ${groupAccount.name}?`)) {
            csharpApi.deleteGroupAccount(groupAccount);
        }
    };

    const handleSave = (e) => {
        e.preventDefault();
        const groupAccountToSave = {
            ...currentGroupAccount,
            categoryId: parseInt(currentGroupAccount.categoryId, 10) || null
        };
        if (groupAccountToSave.id) {
            csharpApi.updateGroupAccount(groupAccountToSave);
        } else {
            csharpApi.addGroupAccount(groupAccountToSave);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setCurrentGroupAccount({ ...currentGroupAccount, [name]: value });
    };

    return (
        <div className="crud-container">
            <div className="card">
                <div className="card-header">
                    <h3>Quản lý Tài khoản Nhóm</h3>
                    <button className="add-new-btn" onClick={handleAddNew}>+ Thêm mới</button>
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
                                        <button className="action-btn edit" onClick={() => handleEdit(groupAccount)}>Sửa</button>
                                        <button className="action-btn delete" onClick={() => handleDelete(groupAccount)}>Xóa</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {isModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h2>{currentGroupAccount && currentGroupAccount.id ? 'Sửa tài khoản nhóm' : 'Thêm tài khoản nhóm'}</h2>
                            <button className="close-btn" onClick={() => setIsModalOpen(false)}>&times;</button>
                        </div>
                        <form onSubmit={handleSave}>
                            <div className="form-group">
                                <label>Tên Nhóm:</label>
                                <input name="name" value={currentGroupAccount?.name || ''} onChange={handleInputChange} required />
                            </div>
                            <div className="form-group">
                                <label>Group ID:</label>
                                <input name="groupId" value={currentGroupAccount?.groupId || ''} onChange={handleInputChange} required />
                            </div>
                            <div className="form-actions">
                                <button type="submit" className="action-btn save">Lưu</button>
                                <button type="button" className="action-btn cancel" onClick={() => setIsModalOpen(false)}>Hủy</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default GroupAccountCrud;
